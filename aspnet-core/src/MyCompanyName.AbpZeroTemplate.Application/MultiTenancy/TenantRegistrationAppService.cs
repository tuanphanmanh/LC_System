using Abp.Application.Features;
using Abp.Application.Services.Dto;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Timing;
using Abp.UI;
using Abp.Zero.Configuration;
using MyCompanyName.AbpZeroTemplate.Configuration;
using MyCompanyName.AbpZeroTemplate.Debugging;
using MyCompanyName.AbpZeroTemplate.Editions;
using MyCompanyName.AbpZeroTemplate.Editions.Dto;
using MyCompanyName.AbpZeroTemplate.Features;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;
using MyCompanyName.AbpZeroTemplate.Notifications;
using MyCompanyName.AbpZeroTemplate.Security.Recaptcha;
using MyCompanyName.AbpZeroTemplate.Url;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Extensions;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy
{
    public class TenantRegistrationAppService : AbpZeroTemplateAppServiceBase, ITenantRegistrationAppService
    {
        public IAppUrlService AppUrlService { get; set; }

        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly IRecaptchaValidator _recaptchaValidator;
        private readonly EditionManager _editionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly ILocalizationContext _localizationContext;
        private readonly TenantManager _tenantManager;
        private readonly IPaymentManager _paymentManager;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IWebUrlService _webUrlService;

        public TenantRegistrationAppService(
            IMultiTenancyConfig multiTenancyConfig,
            IRecaptchaValidator recaptchaValidator,
            EditionManager editionManager,
            IAppNotifier appNotifier,
            ILocalizationContext localizationContext,
            TenantManager tenantManager,
            IPaymentManager paymentManager,
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            IWebUrlService webUrlService)
        {
            _multiTenancyConfig = multiTenancyConfig;
            _recaptchaValidator = recaptchaValidator;
            _editionManager = editionManager;
            _appNotifier = appNotifier;
            _localizationContext = localizationContext;
            _tenantManager = tenantManager;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _webUrlService = webUrlService;
            _paymentManager = paymentManager;

            AppUrlService = NullAppUrlService.Instance;
        }

        public async Task<RegisterTenantOutput> RegisterTenant(RegisterTenantInput input)
        {
            if (input.EditionId.HasValue)
            {
                await CheckEditionSubscriptionAsync(input.EditionId.Value, input.SubscriptionStartType);
            }
            else
            {
                await CheckRegistrationWithoutEdition();
            }

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                CheckTenantRegistrationIsEnabled();

                if (UseCaptchaOnRegistration())
                {
                    await _recaptchaValidator.ValidateAsync(input.CaptchaResponse);
                }

                //Getting host-specific settings
                var isActive = await IsNewRegisteredTenantActiveByDefault(input.SubscriptionStartType);
                var isEmailConfirmationRequired = await SettingManager.GetSettingValueForApplicationAsync<bool>(
                    AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin
                );

                DateTime? subscriptionEndDate = null;
                var isInTrialPeriod = false;

                if (input.EditionId.HasValue)
                {
                    isInTrialPeriod = input.SubscriptionStartType == SubscriptionStartType.Trial;

                    if (isInTrialPeriod)
                    {
                        var edition = (SubscribableEdition) await _editionManager.GetByIdAsync(input.EditionId.Value);
                        subscriptionEndDate = Clock.Now.AddDays(edition.TrialDayCount ?? 0);
                    }
                }

                var tenantId = await _tenantManager.CreateWithAdminUserAsync(
                    input.TenancyName,
                    input.Name,
                    input.AdminPassword,
                    input.AdminEmailAddress,
                    null,
                    isActive,
                    input.EditionId,
                    shouldChangePasswordOnNextLogin: false,
                    sendActivationEmail: true,
                    subscriptionEndDate,
                    isInTrialPeriod,
                    AppUrlService.CreateEmailActivationUrlFormat(input.TenancyName),
                    adminName: input.AdminName,
                    adminSurname: input.AdminSurname
                );

                var tenant = await TenantManager.GetByIdAsync(tenantId);
                await _appNotifier.NewTenantRegisteredAsync(tenant);

                long? paymentId = null;

                if (input.SubscriptionStartType == SubscriptionStartType.Paid)
                {
                    var edition = await _editionManager.GetByIdAsync(input.EditionId.Value) as SubscribableEdition;

                    // Create Payment Request
                    paymentId = await _paymentManager.CreatePayment(new SubscriptionPayment
                    {
                        TenantId = tenantId,
                        PaymentPeriodType = input.PaymentPeriodType,
                        DayCount = (int)input.PaymentPeriodType,
                        SuccessUrl = input.SuccessUrl,
                        ErrorUrl = input.ErrorUrl,
                        ExtraProperties = new ExtraPropertyDictionary
                        {
                            {PaymentConsts.PlanType, PaymentConsts.EditionSubscriptionPlan},
                            {PaymentConsts.PlanId, edition.GetPlanId(input.PaymentPeriodType.Value)}
                        },
                        SubscriptionPaymentProducts =
                        [
                            new SubscriptionPaymentProduct(
                                edition.DisplayName,
                                edition.GetPaymentAmount(input.PaymentPeriodType),
                                1,
                                edition.GetPaymentAmount(input.PaymentPeriodType),
                                new ExtraPropertyDictionary
                                {
                                    {PaymentConsts.TenantId, tenantId.ToString()},
                                    {PaymentConsts.EditionId, edition.Id.ToString()},
                                    {
                                        PaymentConsts.SubscriptionStartType,
                                        ((int) input.SubscriptionStartType).ToString()
                                    }
                                }
                            )
                        ]
                    });
                }

                return new RegisterTenantOutput
                {
                    TenantId = tenant.Id,
                    TenancyName = input.TenancyName,
                    Name = input.Name,
                    UserName = AbpUserBase.AdminUserName,
                    EmailAddress = input.AdminEmailAddress,
                    IsActive = tenant.IsActive,
                    IsEmailConfirmationRequired = isEmailConfirmationRequired,
                    IsTenantActive = tenant.IsActive,
                    PaymentId = paymentId
                };
            }
        }

        public async Task BuyNowSucceed(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetPaymentWithProducts(paymentId);

            if (!payment.SubscriptionPaymentProducts.Any())
            {
                throw new ApplicationException("There is product in this payment !");
            }

            var editionId = payment.GetEditionId();
            var tenantId = payment.TenantId;

            if (AbpSession.GetTenantId() != tenantId)
            {
                throw new ApplicationException("This payment belongs to another tenant !");
            }

            if (payment.Status != SubscriptionPaymentStatus.Paid)
            {
                throw new ApplicationException("Your payment is not completed !");
            }

            if (!editionId.HasValue)
            {
                throw new ApplicationException("There is no edition information in the payment record !");
            }

            payment.SetAsCompleted();

            await _tenantManager.UpdateTenantAsync(
                payment.TenantId,
                true,
                false,
                payment.PaymentPeriodType,
                editionId.Value,
                EditionPaymentType.BuyNow
            );
        }

        public async Task NewRegistrationSucceed(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetPaymentWithProducts(paymentId);
            var editionId = payment.GetEditionId();

            if (payment.Status != SubscriptionPaymentStatus.Paid)
            {
                throw new ApplicationException("Your payment is not completed !");
            }

            payment.SetAsCompleted();

            await _tenantManager.UpdateTenantAsync(
                payment.TenantId,
                true,
                null,
                payment.PaymentPeriodType,
                editionId,
                EditionPaymentType.NewRegistration
            );
        }

        public async Task UpgradeSucceed(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetPaymentWithProducts(paymentId);
            var editionId = payment.GetTargetEditionId();

            if (payment.Status != SubscriptionPaymentStatus.Paid)
            {
                throw new ApplicationException("Your payment is not completed !");
            }

            payment.SetAsCompleted();

            await _tenantManager.UpdateTenantAsync(
                payment.TenantId,
                true,
                null,
                payment.PaymentPeriodType,
                editionId,
                EditionPaymentType.Upgrade
            );
        }

        public async Task ExtendSucceed(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetPaymentWithProducts(paymentId);
            var editionId = payment.SubscriptionPaymentProducts.First().GetProperty<int>("EditionId");

            if (payment.Status != SubscriptionPaymentStatus.Paid)
            {
                throw new ApplicationException("Your payment is not completed !");
            }

            payment.SetAsCompleted();

            await _tenantManager.UpdateTenantAsync(
                payment.TenantId,
                true,
                null,
                payment.PaymentPeriodType,
                editionId,
                EditionPaymentType.Extend
            );
        }

        public async Task<EditionsSelectOutput> GetEditionsForSelect()
        {
            var features = FeatureManager
                .GetAll()
                .Where(feature =>
                    (feature[FeatureMetadata.CustomFeatureKey] as FeatureMetadata)?.IsVisibleOnPricingTable ?? false);

            var flatFeatures = ObjectMapper
                .Map<List<FlatFeatureSelectDto>>(features)
                .OrderBy(f => f.DisplayName)
                .ToList();

            var editions = (await _editionManager.GetAllAsync())
                .Cast<SubscribableEdition>()
                .OrderBy(e => e.MonthlyPrice)
                .ToList();

            var featureDictionary = features.ToDictionary(feature => feature.Name, f => f);

            var editionWithFeatures = new List<EditionWithFeaturesDto>();
            foreach (var edition in editions)
            {
                editionWithFeatures.Add(await CreateEditionWithFeaturesDto(edition, featureDictionary));
            }

            if (AbpSession.UserId.HasValue)
            {
                var currentEditionId = (await _tenantManager.GetByIdAsync(AbpSession.GetTenantId()))
                    .EditionId;

                if (currentEditionId.HasValue)
                {
                    editionWithFeatures = editionWithFeatures.Where(e => e.Edition.Id != currentEditionId).ToList();

                    var currentEdition =
                        (SubscribableEdition) (await _editionManager.GetByIdAsync(currentEditionId.Value));
                    if (!currentEdition.IsFree)
                    {
                        var lastPayment = await _subscriptionPaymentRepository.GetLastCompletedPaymentOrDefaultAsync(
                            AbpSession.GetTenantId(),
                            null,
                            null);

                        if (lastPayment != null)
                        {
                            editionWithFeatures = editionWithFeatures
                                .Where(e =>
                                    e.Edition.GetPaymentAmount(lastPayment.PaymentPeriodType) >
                                    currentEdition.GetPaymentAmount(lastPayment.PaymentPeriodType)
                                )
                                .ToList();
                        }
                    }
                }
            }

            return new EditionsSelectOutput
            {
                AllFeatures = flatFeatures,
                EditionsWithFeatures = editionWithFeatures,
            };
        }

        public async Task<EditionSelectDto> GetEdition(int editionId)
        {
            var edition = await _editionManager.GetByIdAsync(editionId);
            var editionDto = ObjectMapper.Map<EditionSelectDto>(edition);

            return editionDto;
        }

        private async Task<bool> IsNewRegisteredTenantActiveByDefault(SubscriptionStartType subscriptionStartType)
        {
            if (subscriptionStartType == SubscriptionStartType.Paid)
            {
                return false;
            }

            return await SettingManager.GetSettingValueForApplicationAsync<bool>(AppSettings.TenantManagement
                .IsNewRegisteredTenantActiveByDefault);
        }

        private async Task CheckRegistrationWithoutEdition()
        {
            var editions = await _editionManager.GetAllAsync();
            if (editions.Any())
            {
                throw new Exception(
                    "Tenant registration is not allowed without edition because there are editions defined !");
            }
        }

        private async Task<EditionWithFeaturesDto> CreateEditionWithFeaturesDto(SubscribableEdition edition,
            Dictionary<string, Feature> featureDictionary)
        {
            return new EditionWithFeaturesDto
            {
                Edition = ObjectMapper.Map<EditionSelectDto>(edition),
                FeatureValues = (await _editionManager.GetFeatureValuesAsync(edition.Id))
                    .Where(featureValue => featureDictionary.ContainsKey(featureValue.Name))
                    .Select(fv => new NameValueDto(
                        fv.Name,
                        featureDictionary[fv.Name].GetValueText(fv.Value, _localizationContext))
                    )
                    .ToList()
            };
        }

        private void CheckTenantRegistrationIsEnabled()
        {
            if (!IsSelfRegistrationEnabled())
            {
                throw new UserFriendlyException(L("SelfTenantRegistrationIsDisabledMessage_Detail"));
            }

            if (!_multiTenancyConfig.IsEnabled)
            {
                throw new UserFriendlyException(L("MultiTenancyIsNotEnabled"));
            }
        }

        private bool IsSelfRegistrationEnabled()
        {
            return SettingManager.GetSettingValueForApplication<bool>(
                AppSettings.TenantManagement.AllowSelfRegistration);
        }

        private bool UseCaptchaOnRegistration()
        {
            return SettingManager.GetSettingValueForApplication<bool>(AppSettings.TenantManagement
                .UseCaptchaOnRegistration);
        }

        private async Task CheckEditionSubscriptionAsync(int editionId, SubscriptionStartType subscriptionStartType)
        {
            var edition = await _editionManager.GetByIdAsync(editionId) as SubscribableEdition;

            CheckSubscriptionStart(edition, subscriptionStartType);
        }

        private static void CheckSubscriptionStart(SubscribableEdition edition,
            SubscriptionStartType subscriptionStartType)
        {
            switch (subscriptionStartType)
            {
                case SubscriptionStartType.Free:
                    if (!edition.IsFree)
                    {
                        throw new Exception("This is not a free edition !");
                    }

                    break;
                case SubscriptionStartType.Trial:
                    if (!edition.HasTrial())
                    {
                        throw new Exception("Trial is not available for this edition !");
                    }

                    break;
                case SubscriptionStartType.Paid:
                    if (edition.IsFree)
                    {
                        throw new Exception("This is a free edition and cannot be subscribed as paid !");
                    }

                    break;
            }
        }
    }
}