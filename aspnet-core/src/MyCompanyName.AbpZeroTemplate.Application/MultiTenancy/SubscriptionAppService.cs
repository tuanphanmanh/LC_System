using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Events.Bus;
using Abp.Runtime.Session;
using Abp.UI;
using MyCompanyName.AbpZeroTemplate.Editions;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy
{
    public class SubscriptionAppService : AbpZeroTemplateAppServiceBase, ISubscriptionAppService
    {
        public IEventBus EventBus { get; set; }

        private readonly EditionManager _editionManager;
        private readonly TenantManager _tenantManager;
        private readonly IPaymentManager _paymentManager;

        public SubscriptionAppService(
            EditionManager editionManager,
            TenantManager tenantManager,
            IPaymentManager paymentManager)
        {
            _editionManager = editionManager;
            _tenantManager = tenantManager;
            _paymentManager = paymentManager;

            EventBus = NullEventBus.Instance;
        }

        public async Task DisableRecurringPayments()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await TenantManager.GetByIdAsync(AbpSession.GetTenantId());
                if (!tenant.EditionId.HasValue)
                {
                    throw new ApplicationException("This tenant has no edition. Can not disable recurring payments!");
                }

                var edition = await _editionManager.GetByIdAsync(tenant.EditionId.Value) as SubscribableEdition;
                if (edition == null)
                {
                    throw new ApplicationException("This tenant has no edition. Can not disable recurring payments!");
                }

                var daysUntilDue = edition.WaitingDayAfterExpire ?? 3;

                if (tenant.SubscriptionPaymentType == SubscriptionPaymentType.RecurringAutomatic)
                {
                    tenant.SubscriptionPaymentType = SubscriptionPaymentType.RecurringManual;
                    await EventBus.TriggerAsync(
                        new RecurringPaymentsDisabledEventData(AbpSession.GetTenantId(), daysUntilDue)
                    );
                }
            }
        }

        public async Task EnableRecurringPayments()
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await TenantManager.GetByIdAsync(AbpSession.GetTenantId());
                if (tenant.SubscriptionPaymentType == SubscriptionPaymentType.RecurringManual)
                {
                    tenant.SubscriptionPaymentType = SubscriptionPaymentType.RecurringAutomatic;

                    await EventBus.TriggerAsync(new RecurringPaymentsEnabledEventData
                    {
                        TenantId = AbpSession.GetTenantId()
                    });
                }
            }
        }

        public async Task<long> StartExtendSubscription(StartExtendSubscriptionInput input)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await TenantManager.GetByIdAsync(AbpSession.GetTenantId());

                if (!tenant.EditionId.HasValue)
                {
                    throw new UserFriendlyException("Your tenant doesn't have any edition.");
                }

                var edition = await _editionManager.GetByIdAsync(tenant.EditionId.Value) as SubscribableEdition;
                var paymentPeriodType = await _tenantManager.GetCurrentPaymentPeriodType(AbpSession.GetTenantId());

                return await _paymentManager.CreatePayment(new SubscriptionPayment
                {
                    TenantId = tenant.Id,
                    SuccessUrl = input.SuccessUrl,
                    ErrorUrl = input.ErrorUrl,
                    PaymentPeriodType = paymentPeriodType,
                    DayCount = (int) paymentPeriodType,
                    IsRecurring = false,
                    SubscriptionPaymentProducts = new List<SubscriptionPaymentProduct>
                    {
                        new(
                            description:
                            $"Extend subscription {Convert.ToInt32(paymentPeriodType)} days for {edition.DisplayName}",
                            edition.GetPaymentAmount(paymentPeriodType),
                            count: 1,
                            extraProperties: new ExtraPropertyDictionary
                            {
                                {PaymentConsts.TenantId, tenant.Id.ToString()},
                                {PaymentConsts.EditionId, edition.Id.ToString()},
                                {PaymentConsts.PaymentPeriodType, paymentPeriodType.ToString()}
                            }
                        )
                    }
                });
            }
        }

        public async Task<StartUpgradeSubscriptionOutput> StartUpgradeSubscription(StartUpgradeSubscriptionInput input)
        {
            if (!AbpSession.TenantId.HasValue)
            {
                throw new ArgumentNullException();
            }

            Tenant tenant;
            SubscribableEdition targetEdition;
            PaymentPeriodType currentPaymentPeriodType;

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                tenant = await _tenantManager.GetByIdAsync(AbpSession.GetTenantId());
                targetEdition = await _editionManager.GetByIdAsync(input.TargetEditionId) as SubscribableEdition;
                if (!input.PaymentPeriodType.HasValue)
                {
                    currentPaymentPeriodType = await _tenantManager.GetCurrentPaymentPeriodType(AbpSession.GetTenantId());
                }
                else
                {
                    currentPaymentPeriodType = input.PaymentPeriodType.Value;
                }

                if (tenant.EditionId.HasValue)
                {
                    var currentEdition = await _editionManager.GetByIdAsync(tenant.EditionId.Value);
                    if (((SubscribableEdition) currentEdition).IsFree)
                    {
                        if (targetEdition.IsFree)
                        {
                            await _tenantManager.SwitchBetweenFreeEditions(input.TargetEditionId);
                            return new StartUpgradeSubscriptionOutput(true);
                        }

                        var paymentId = await CreateUpgradeSubscriptionPayment(tenant, input, currentPaymentPeriodType);
                        return new StartUpgradeSubscriptionOutput(false, paymentId);
                    }

                    using (CurrentUnitOfWork.SetTenantId(AbpSession.GetTenantId()))
                    {
                        if (!await _paymentManager.HasAnyPayment(AbpSession.GetTenantId()))
                        {
                            var paymentId = await CreateUpgradeSubscriptionPayment(tenant, input, currentPaymentPeriodType);
                            return new StartUpgradeSubscriptionOutput(false, paymentId);
                        }
                    }
                }
            }
            
            var upgradeAmount = await CalculateAmountForPaymentAsync(tenant, targetEdition, currentPaymentPeriodType);

            if (IsLessThanMinimumUpgradePaymentAmount(upgradeAmount))
            {
                await _tenantManager.UpdateTenantAsync(
                    AbpSession.GetTenantId(),
                    tenant.IsActive,
                    tenant.IsInTrialPeriod,
                    currentPaymentPeriodType,
                    input.TargetEditionId,
                    EditionPaymentType.Upgrade
                );

                return new StartUpgradeSubscriptionOutput(true);
            }

            var upgradePaymentId = await CreateUpgradeSubscriptionPayment(tenant, input, currentPaymentPeriodType);
            return new StartUpgradeSubscriptionOutput(false, upgradePaymentId);
        }

        public async Task<long> StartTrialToBuySubscription(StartTrialToBuySubscriptionInput input)
        {
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await TenantManager.GetByIdAsync(AbpSession.GetTenantId());

                if (!tenant.EditionId.HasValue)
                {
                    throw new UserFriendlyException("Your tenant doesn't have any edition.");
                }

                var edition = await _editionManager.GetByIdAsync(tenant.EditionId.Value) as SubscribableEdition;

                return await _paymentManager.CreatePayment(new SubscriptionPayment
                {
                    TenantId = tenant.Id,
                    SuccessUrl = input.SuccessUrl,
                    ErrorUrl = input.ErrorUrl,
                    PaymentPeriodType = input.PaymentPeriodType,
                    DayCount = (int) input.PaymentPeriodType,
                    IsRecurring = false,
                    SubscriptionPaymentProducts = new List<SubscriptionPaymentProduct>
                    {
                        new(
                            description: edition.DisplayName,
                            edition.GetPaymentAmount(input.PaymentPeriodType),
                            count: 1,
                            extraProperties: new ExtraPropertyDictionary
                            {
                                {PaymentConsts.TenantId, tenant.Id.ToString()},
                                {PaymentConsts.EditionId, edition.Id.ToString()},
                                {PaymentConsts.PaymentPeriodType, input.PaymentPeriodType.ToString()}
                            }
                        )
                    }
                });
            }
        }

        private bool IsLessThanMinimumUpgradePaymentAmount(decimal upgradeAmount)
        {
            return upgradeAmount < AbpZeroTemplateConsts.MinimumUpgradePaymentAmount;
        }

        private async Task<decimal> CalculateAmountForPaymentAsync(
            Tenant tenant,
            SubscribableEdition targetEdition,
            PaymentPeriodType periodType)
        {
            if (tenant.EditionId == null)
            {
                throw new UserFriendlyException(L("CanNotUpgradeSubscriptionSinceTenantHasNoEditionAssigned"));
            }

            var remainingHoursCount = tenant.CalculateRemainingHoursCount();

            if (remainingHoursCount <= 0)
            {
                return targetEdition.GetPaymentAmount(periodType);
            }

            var currentEdition = (SubscribableEdition) await _editionManager.GetByIdAsync(tenant.EditionId.Value);

            return TenantManager.GetUpgradePrice(
                currentEdition,
                targetEdition,
                remainingHoursCount,
                periodType
            );
        }

        private async Task<long> CreateUpgradeSubscriptionPayment(Tenant tenant, StartUpgradeSubscriptionInput input, PaymentPeriodType paymentPeriodType)
        {
            var targetEdition = (SubscribableEdition) await _editionManager.GetByIdAsync(input.TargetEditionId);

            var upgradeAmount = await CalculateAmountForPaymentAsync(tenant, targetEdition, paymentPeriodType);
            var payment = new SubscriptionPayment
            {
                TenantId = tenant.Id,
                SuccessUrl = input.SuccessUrl,
                ErrorUrl = input.ErrorUrl,
                PaymentPeriodType = paymentPeriodType,
                DayCount = (int) paymentPeriodType,
                // If the tenant is on a recurring payment plan and operation is upgrade, then it is a proration payment.
                IsProrationPayment = tenant.SubscriptionPaymentType == SubscriptionPaymentType.RecurringAutomatic,
                IsRecurring = tenant.SubscriptionPaymentType == SubscriptionPaymentType.RecurringAutomatic,
                SubscriptionPaymentProducts = new List<SubscriptionPaymentProduct>()
                {
                    new(
                        description: $"Upgrade to {targetEdition.DisplayName} edition",
                        amount: upgradeAmount,
                        count: 1,
                        extraProperties: new ExtraPropertyDictionary
                        {
                            {PaymentConsts.TenantId, tenant.Id.ToString()},
                            {PaymentConsts.TargetEditionId, targetEdition.Id.ToString()},
                            {PaymentConsts.PaymentPeriodType, paymentPeriodType.ToString()},
                        }
                    )
                }
            };

            if (tenant.SubscriptionPaymentType == SubscriptionPaymentType.RecurringAutomatic)
            {
                payment.SetProperty(PaymentConsts.PlanId, targetEdition.GetPlanId(paymentPeriodType));
                payment.SetProperty(PaymentConsts.PlanAmount, targetEdition.GetPaymentAmount(paymentPeriodType));
                payment.SetProperty(PaymentConsts.PlanType, PaymentConsts.EditionSubscriptionPlan);
            }

            return await _paymentManager.CreatePayment(payment);
        }
    }
}