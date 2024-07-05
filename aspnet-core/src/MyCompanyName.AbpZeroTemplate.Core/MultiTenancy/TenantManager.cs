using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Abp;
using Abp.Application.Features;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.MultiTenancy;
using MyCompanyName.AbpZeroTemplate.Authorization.Roles;
using MyCompanyName.AbpZeroTemplate.Authorization.Users;
using MyCompanyName.AbpZeroTemplate.Editions;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Demo;
using Abp.Extensions;
using Abp.Notifications;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using MyCompanyName.AbpZeroTemplate.Notifications;
using System;
using System.Diagnostics;
using Abp.BackgroundJobs;
using Abp.Events.Bus;
using Abp.Events.Bus.Handlers;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.UI;
using Castle.Core.Logging;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy
{
    /// <summary>
    /// Tenant manager.
    /// </summary>
    public class TenantManager :
        AbpTenantManager<Tenant, User>,
        IEventHandler<TenantEditionChangedEventData>,
        IEventHandler<RecurringPaymentSucceedEventData>
    {
        public IAbpSession AbpSession { get; set; }
        public IEventBus EventBus { get; set; }

        public ILogger Logger { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly RoleManager _roleManager;
        private readonly UserManager _userManager;
        private readonly IUserEmailer _userEmailer;
        private readonly INotificationSubscriptionManager _notificationSubscriptionManager;
        private readonly IAppNotifier _appNotifier;
        private readonly IAbpZeroDbMigrator _abpZeroDbMigrator;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IRepository<SubscribableEdition> _subscribableEditionRepository;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly EditionManager _editionManager;
        private readonly IPaymentManager _paymentManager;

        protected readonly IBackgroundJobManager _backgroundJobManager;


        public TenantManager(
            IRepository<Tenant> tenantRepository,
            IRepository<TenantFeatureSetting, long> tenantFeatureRepository,
            IUnitOfWorkManager unitOfWorkManager,
            RoleManager roleManager,
            IUserEmailer userEmailer,
            UserManager userManager,
            INotificationSubscriptionManager notificationSubscriptionManager,
            IAppNotifier appNotifier,
            IAbpZeroFeatureValueStore featureValueStore,
            IAbpZeroDbMigrator abpZeroDbMigrator,
            IPasswordHasher<User> passwordHasher,
            IRepository<SubscribableEdition> subscribableEditionRepository,
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            EditionManager editionManager,
            IBackgroundJobManager backgroundJobManager, IPaymentManager paymentManager) : base(
            tenantRepository,
            tenantFeatureRepository,
            editionManager,
            featureValueStore
        )
        {
            AbpSession = NullAbpSession.Instance;
            EventBus = NullEventBus.Instance;
            Logger = NullLogger.Instance;

            _editionManager = editionManager;
            _unitOfWorkManager = unitOfWorkManager;
            _roleManager = roleManager;
            _userEmailer = userEmailer;
            _userManager = userManager;
            _notificationSubscriptionManager = notificationSubscriptionManager;
            _appNotifier = appNotifier;
            _abpZeroDbMigrator = abpZeroDbMigrator;
            _passwordHasher = passwordHasher;
            _subscribableEditionRepository = subscribableEditionRepository;
            _backgroundJobManager = backgroundJobManager;
            _paymentManager = paymentManager;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
        }

        public async Task<int> CreateWithAdminUserAsync(
            string tenancyName,
            string name,
            string adminPassword,
            string adminEmailAddress,
            string connectionString,
            bool isActive,
            int? editionId,
            bool shouldChangePasswordOnNextLogin,
            bool sendActivationEmail,
            DateTime? subscriptionEndDate,
            bool isInTrialPeriod,
            string emailActivationLink,
            string adminName = null,
            string adminSurname = null
        )
        {
            int newTenantId;
            long newAdminId;

            await CheckEditionAsync(editionId, isInTrialPeriod);

            if (isInTrialPeriod && !subscriptionEndDate.HasValue)
            {
                throw new UserFriendlyException(LocalizationManager.GetString(
                    AbpZeroTemplateConsts.LocalizationSourceName, "TrialWithoutEndDateErrorMessage"));
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                //Create tenant
                var tenant = new Tenant(tenancyName, name)
                {
                    IsActive = isActive,
                    EditionId = editionId,
                    SubscriptionEndDateUtc = subscriptionEndDate?.ToUniversalTime(),
                    IsInTrialPeriod = isInTrialPeriod,
                    ConnectionString = connectionString.IsNullOrWhiteSpace()
                        ? null
                        : SimpleStringCipher.Instance.Encrypt(connectionString)
                };

                await CreateAsync(tenant);
                await _unitOfWorkManager.Current.SaveChangesAsync(); //To get new tenant's id.

                //Create tenant database
                _abpZeroDbMigrator.CreateOrMigrateForTenant(tenant);

                //We are working entities of new tenant, so changing tenant filter
                using (_unitOfWorkManager.Current.SetTenantId(tenant.Id))
                {
                    //Create static roles for new tenant
                    CheckErrors(await _roleManager.CreateStaticRoles(tenant.Id));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get static role ids

                    //grant all permissions to admin role
                    var adminRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.Admin);
                    await _roleManager.GrantAllPermissionsAsync(adminRole);

                    //User role should be default
                    var userRole = _roleManager.Roles.Single(r => r.Name == StaticRoleNames.Tenants.User);
                    userRole.IsDefault = true;
                    CheckErrors(await _roleManager.UpdateAsync(userRole));

                    //Create admin user for the tenant
                    var adminUser = User.CreateTenantAdminUser(tenant.Id, adminEmailAddress, adminName, adminSurname);
                    adminUser.ShouldChangePasswordOnNextLogin = shouldChangePasswordOnNextLogin;
                    adminUser.IsActive = true;

                    if (adminPassword.IsNullOrEmpty())
                    {
                        adminPassword = await _userManager.CreateRandomPassword();
                    }
                    else
                    {
                        await _userManager.InitializeOptionsAsync(AbpSession.TenantId);
                        foreach (var validator in _userManager.PasswordValidators)
                        {
                            CheckErrors(await validator.ValidateAsync(_userManager, adminUser, adminPassword));
                        }
                    }

                    adminUser.Password = _passwordHasher.HashPassword(adminUser, adminPassword);

                    CheckErrors(await _userManager.CreateAsync(adminUser));
                    await _unitOfWorkManager.Current.SaveChangesAsync(); //To get admin user's id

                    //Assign admin user to admin role!
                    CheckErrors(await _userManager.AddToRoleAsync(adminUser, adminRole.Name));

                    //Notifications
                    await _appNotifier.WelcomeToTheApplicationAsync(adminUser);

                    //Send activation email
                    if (sendActivationEmail)
                    {
                        adminUser.SetNewEmailConfirmationCode();
                        await _userEmailer.SendEmailActivationLinkAsync(adminUser, emailActivationLink, adminPassword);
                    }

                    await _unitOfWorkManager.Current.SaveChangesAsync();

                    await _backgroundJobManager.EnqueueAsync<TenantDemoDataBuilderJob, int>(tenant.Id);

                    newTenantId = tenant.Id;
                    newAdminId = adminUser.Id;
                }

                await uow.CompleteAsync();
            }

            //Used a second UOW since UOW above sets some permissions and _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync needs these permissions to be saved.
            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                using (_unitOfWorkManager.Current.SetTenantId(newTenantId))
                {
                    await _notificationSubscriptionManager.SubscribeToAllAvailableNotificationsAsync(
                        new UserIdentifier(newTenantId, newAdminId));
                    await _unitOfWorkManager.Current.SaveChangesAsync();
                    await uow.CompleteAsync();
                }
            }

            return newTenantId;
        }

        public async Task CheckEditionAsync(int? editionId, bool isInTrialPeriod)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                if (!editionId.HasValue || !isInTrialPeriod)
                {
                    return;
                }

                var edition = await _subscribableEditionRepository.GetAsync(editionId.Value);
                if (!edition.IsFree)
                {
                    return;
                }

                var error = LocalizationManager.GetSource(AbpZeroTemplateConsts.LocalizationSourceName)
                    .GetString("FreeEditionsCannotHaveTrialVersions");
                throw new UserFriendlyException(error);
            });
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }

        public decimal GetUpgradePrice(SubscribableEdition currentEdition, SubscribableEdition targetEdition,
            int totalRemainingHourCount, PaymentPeriodType paymentPeriodType)
        {
            int numberOfHoursPerDay = 24;

            var totalRemainingDayCount = totalRemainingHourCount / numberOfHoursPerDay;
            var unusedPeriodCount = totalRemainingDayCount / (int) paymentPeriodType;
            var unusedHoursCount = totalRemainingHourCount % ((int) paymentPeriodType * numberOfHoursPerDay);

            decimal currentEditionPriceForUnusedPeriod = 0;
            decimal targetEditionPriceForUnusedPeriod = 0;

            var currentEditionPrice = currentEdition.GetPaymentAmount(paymentPeriodType);
            var targetEditionPrice = targetEdition.GetPaymentAmount(paymentPeriodType);

            if (currentEditionPrice > 0)
            {
                currentEditionPriceForUnusedPeriod = currentEditionPrice * unusedPeriodCount;
                currentEditionPriceForUnusedPeriod += (currentEditionPrice / (int) paymentPeriodType) /
                    numberOfHoursPerDay * unusedHoursCount;
            }

            if (targetEditionPrice > 0)
            {
                targetEditionPriceForUnusedPeriod = targetEditionPrice * unusedPeriodCount;
                targetEditionPriceForUnusedPeriod += (targetEditionPrice / (int) paymentPeriodType) /
                    numberOfHoursPerDay * unusedHoursCount;
            }

            return targetEditionPriceForUnusedPeriod - currentEditionPriceForUnusedPeriod;
        }

        public async Task<Tenant> UpdateTenantAsync(
            int tenantId,
            bool isActive,
            bool? isInTrialPeriod,
            PaymentPeriodType? paymentPeriodType,
            int? editionId,
            EditionPaymentType editionPaymentType)
        {
            var tenant = await FindByIdAsync(tenantId);

            tenant.IsActive = isActive;
            tenant.EditionId = editionId;

            if (isInTrialPeriod.HasValue)
            {
                tenant.IsInTrialPeriod = isInTrialPeriod.Value;
            }

            if (paymentPeriodType.HasValue)
            {
                tenant.UpdateSubscriptionDateForPayment(paymentPeriodType.Value, editionPaymentType);
            }

            return tenant;
        }

        public async Task<EndSubscriptionResult> EndSubscriptionAsync(Tenant tenant, SubscribableEdition edition,
            DateTime nowUtc)
        {
            if (tenant.EditionId == null || tenant.HasUnlimitedTimeSubscription())
            {
                throw new Exception(
                    $"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} tenant has unlimited time subscription!");
            }

            Debug.Assert(tenant.SubscriptionEndDateUtc != null, "tenant.SubscriptionEndDateUtc != null");

            var subscriptionEndDateUtc = tenant.SubscriptionEndDateUtc.Value;
            if (!tenant.IsInTrialPeriod)
            {
                subscriptionEndDateUtc =
                    tenant.SubscriptionEndDateUtc.Value.AddDays(edition.WaitingDayAfterExpire ?? 0);
            }

            if (subscriptionEndDateUtc >= nowUtc)
            {
                throw new Exception(
                    $"Can not end tenant {tenant.TenancyName} subscription for {edition.DisplayName} since subscription has not expired yet!");
            }

            if (!tenant.IsInTrialPeriod && edition.ExpiringEditionId.HasValue)
            {
                tenant.EditionId = edition.ExpiringEditionId.Value;
                tenant.SubscriptionEndDateUtc = null;

                await UpdateAsync(tenant);

                return EndSubscriptionResult.AssignedToAnotherEdition;
            }

            tenant.IsActive = false;
            tenant.IsInTrialPeriod = false;

            await UpdateAsync(tenant);

            return EndSubscriptionResult.TenantSetInActive;
        }

        public override Task UpdateAsync(Tenant tenant)
        {
            if (tenant.IsInTrialPeriod && !tenant.SubscriptionEndDateUtc.HasValue)
            {
                throw new UserFriendlyException(LocalizationManager.GetString(
                    AbpZeroTemplateConsts.LocalizationSourceName, "TrialWithoutEndDateErrorMessage"));
            }

            return base.UpdateAsync(tenant);
        }

        public async Task<PaymentPeriodType> GetCurrentPaymentPeriodType(int tenantId)
        {
            var lastPayment =
                await _subscriptionPaymentRepository.GetLastCompletedPaymentOrDefaultAsync(tenantId, null, null);
            if (lastPayment == null)
            {
                return PaymentPeriodType.Monthly;
            }

            return lastPayment.PaymentPeriodType ?? PaymentPeriodType.Monthly;
        }

        public async Task SwitchBetweenFreeEditions(int upgradeEditionId)
        {
            var tenant = await GetByIdAsync(AbpSession.GetTenantId());

            if (!tenant.EditionId.HasValue)
            {
                throw new ArgumentException("tenant.EditionId can not be null");
            }

            var currentEdition = await _editionManager.GetByIdAsync(tenant.EditionId.Value);
            if (!((SubscribableEdition) currentEdition).IsFree)
            {
                throw new ArgumentException("You can only switch between free editions. Current edition if not free");
            }

            var upgradeEdition = await _editionManager.GetByIdAsync(upgradeEditionId);
            if (!((SubscribableEdition) upgradeEdition).IsFree)
            {
                throw new ArgumentException("You can only switch between free editions. Target edition if not free");
            }

            await UpdateTenantAsync(
                tenant.Id,
                true,
                null,
                null,
                upgradeEditionId,
                EditionPaymentType.Upgrade
            );
        }

        public void HandleEvent(TenantEditionChangedEventData eventData)
        {
            if (!eventData.OldEditionId.HasValue)
            {
                return;
            }

            var lastPayment = _paymentManager.GetLastCompletedSubscriptionPayment(
                eventData.TenantId
            );

            if (lastPayment == null)
            {
                return;
            }

            if (!eventData.NewEditionId.HasValue)
            {
                EventBus.Trigger(new SubscriptionCancelledEventData
                {
                    PaymentId = lastPayment.Id,
                    ExternalPaymentId = lastPayment.ExternalPaymentId
                });
                return;
            }

            var edition = _editionManager.GetById(eventData.NewEditionId.Value) as SubscribableEdition;
            var paymentPeriodType = lastPayment.PaymentPeriodType.Value;

            EventBus.Trigger(new SubscriptionUpdatedEventData
            {
                TenantId = eventData.TenantId,
                PaymentId = lastPayment.Id,
                ExternalPaymentId = lastPayment.ExternalPaymentId,
                NewPlanId = edition.GetPlanId(paymentPeriodType),
                NewPlanAmount = edition.GetPaymentAmountOrNull(paymentPeriodType),
                Description = $"Edition change by admin from {eventData.OldEditionId} to {eventData.NewEditionId}",
                ExtraProperties = new ExtraPropertyDictionary
                {
                    [PaymentConsts.OldEditionId] = eventData.OldEditionId.Value,
                    [PaymentConsts.NewEditionId] = eventData.NewEditionId.Value,
                }
            });
        }

        public void HandleEvent(RecurringPaymentSucceedEventData eventData)
        {
            _unitOfWorkManager.WithUnitOfWork(() =>
            {
                if (!eventData.Items.Any())
                {
                    Logger.Warn("There is no item in the recurring payment. " +
                                "Ignoring the payment: " + eventData.ExternalPaymentId);

                    return;
                }

                var editionItem = eventData.Items.FirstOrDefault();
                if (editionItem == null || !editionItem.Product.Equals(AbpZeroTemplateConsts.ProductName))
                {
                    Logger.Warn(
                        $"This is not a payment for {AbpZeroTemplateConsts.ProductName} for edition subscription cycle! " +
                        $"Ignoring payment: " + eventData.ExternalPaymentId
                    );

                    return;
                }
                
                if (editionItem.PlanType != PaymentConsts.EditionSubscriptionPlan)
                {
                    Logger.Warn(
                        $"This is not a payment for {PaymentConsts.EditionSubscriptionPlan} for edition subscription cycle! " +
                        $"Ignoring payment: " + eventData.ExternalPaymentId
                    );
                    return;
                }

                var editionItemValues = editionItem.PlanId.Split('_');
                var editionName = editionItemValues[0];
                var paymentPeriodType = Enum.Parse<PaymentPeriodType>(editionItemValues[1]);

                int? editionId;
                string editionDisplayName;

                using (_unitOfWorkManager.Current.SetTenantId(eventData.TenantId))
                {
                    var edition = _editionManager.FindByName(editionName);
                    editionId = edition?.Id;
                    editionDisplayName = edition?.DisplayName;
                }

                if (!editionId.HasValue)
                {
                    throw new ApplicationException("There is no edition for this recurring payment !");
                }

                AsyncHelper.RunSync(() =>
                    UpdateTenantAsync(
                        eventData.TenantId,
                        isActive: true,
                        isInTrialPeriod: false,
                        paymentPeriodType,
                        editionId.Value,
                        EditionPaymentType.Extend
                    )
                );

                var payment = new SubscriptionPayment
                {
                    TenantId = eventData.TenantId,
                    DayCount = (int) paymentPeriodType,
                    PaymentPeriodType = paymentPeriodType,
                    ExternalPaymentId = eventData.ExternalPaymentId,
                    Gateway = SubscriptionPaymentGatewayType.Stripe,
                    IsRecurring = true,
                    SubscriptionPaymentProducts =
                    {
                        new SubscriptionPaymentProduct(
                            $"Extend subscription {Convert.ToInt32(paymentPeriodType)} days for {editionDisplayName}", 
                            editionItem.Amount, 
                            1, 
                            totalAmount: editionItem.Amount
                        )
                    }
                };

                payment.SetProperty(PaymentConsts.PlanId, editionItem.PlanId);
                payment.SetAsPaid();

                _subscriptionPaymentRepository.Insert(payment);
            });
        }
    }
}