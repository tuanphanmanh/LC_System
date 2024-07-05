using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Events.Bus;
using Abp.Extensions;
using Abp.Threading;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription;
using Stripe;
using Stripe.Checkout;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe
{
    public class StripeGatewayManager : AbpZeroTemplateServiceBase,
        ISupportsRecurringPayments,
        ITransientDependency
    {
        private readonly TenantManager _tenantManager;
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly IPaymentManager _paymentManager;

        public static string StripeSessionIdSubscriptionPaymentExtensionDataKey = "StripeSessionId";

        public IEventBus EventBus { get; set; }

        public StripeGatewayManager(
            TenantManager tenantManager,
            ISubscriptionPaymentRepository subscriptionPaymentRepository,
            IPaymentManager paymentManager)
        {
            EventBus = NullEventBus.Instance;

            _tenantManager = tenantManager;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
            _paymentManager = paymentManager;
        }

        public async Task UpdateSubscription(SubscriptionPayment payment, bool isProrateCharged = false)
        {
            var lastPayment = await _subscriptionPaymentRepository.GetLastCompletedPaymentOrDefaultAsync(
                tenantId: payment.TenantId,
                SubscriptionPaymentGatewayType.Stripe,
                isRecurring: true);

            var newPlanId = payment.GetPlanId();
            decimal? newPlanAmount = payment.GetPlanAmount();

            if (!(await DoesPlanExistAsync(newPlanId)))
            {
                await CreatePlanAsync(
                    newPlanId,
                    payment.GetPlanType(),
                    newPlanAmount.Value,
                    GetPlanInterval(payment.PaymentPeriodType),
                    AbpZeroTemplateConsts.ProductName
                );
            }

            await UpdateSubscription(lastPayment.ExternalPaymentId, newPlanId, isProrateCharged);
        }

        private async Task UpdateSubscription(string subscriptionId, string newPlanId, bool isProrateCharged = false)
        {
            var subscriptionService = new SubscriptionService();
            var subscription = await subscriptionService.GetAsync(subscriptionId);

            await subscriptionService.UpdateAsync(subscriptionId, new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                Items =
                [
                    new SubscriptionItemOptions
                    {
                        Id = subscription.Items.Data[0].Id,
                        Plan = newPlanId
                    }
                ],
                ProrationBehavior = !isProrateCharged ? "always_invoice" : "none"
            });

            var lastRecurringPayment = await _subscriptionPaymentRepository.GetByGatewayAndPaymentIdAsync(
                SubscriptionPaymentGatewayType.Stripe, subscriptionId
            );

            var payment = await _subscriptionPaymentRepository.GetLastPaymentOrDefaultAsync(
                tenantId: lastRecurringPayment.TenantId,
                SubscriptionPaymentGatewayType.Stripe,
                isRecurring: true);

            // This is a one-time payment to upgrade subscription!
            payment.IsRecurring = false;
        }

        public async Task CancelSubscription(string subscriptionId)
        {
            var subscriptionService = new SubscriptionService();
            await subscriptionService.CancelAsync(subscriptionId, null);

            var payment = await _subscriptionPaymentRepository.GetByGatewayAndPaymentIdAsync(
                SubscriptionPaymentGatewayType.Stripe,
                subscriptionId
            );

            payment.SetAsCancelled();
        }

        public async Task<bool> DoesPlanExistAsync(string planId)
        {
            try
            {
                var planService = new PlanService();
                await planService.GetAsync(planId);

                return true;
            }
            catch (StripeException)
            {
                return false;
            }
        }

        public async Task<StripeIdResponse> GetOrCreatePlanAsync(
            string planId,
            string planType,
            decimal amount,
            string interval,
            string productId)
        {
            try
            {
                var planService = new PlanService();
                var plan = await planService.GetAsync(planId);

                return new StripeIdResponse
                {
                    Id = plan.Id
                };
            }
            catch (StripeException)
            {
                return await CreatePlanAsync(planId, planType, amount, interval, productId);
            }
        }

        public async Task<StripeIdResponse> GetOrCreateProductAsync(string productId)
        {
            try
            {
                var productService = new ProductService();
                var product = await productService.GetAsync(productId);

                return new StripeIdResponse
                {
                    Id = product.Id
                };
            }
            catch (StripeException exception)
            {
                Logger.Error(exception.Message, exception);
                return await CreateProductAsync(productId);
            }
        }

        public string GetPlanInterval(PaymentPeriodType? paymentPeriod)
        {
            if (!paymentPeriod.HasValue)
            {
                throw new ArgumentNullException(nameof(paymentPeriod));
            }

            switch (paymentPeriod.Value)
            {
                case PaymentPeriodType.Monthly:
                    return "month";
                case PaymentPeriodType.Annual:
                    return "year";
                default:
                    throw new NotImplementedException($"The plan interval for {paymentPeriod} is not implemented");
            }
        }

        public async Task<StripeIdResponse> UpdateCustomerDescriptionAsync(string sessionId, string description)
        {
            var sessionService = new SessionService();
            var session = await sessionService.GetAsync(sessionId);

            var customer = await CreateOrUpdateCustomerAsync(session, description);

            return new StripeIdResponse
            {
                Id = customer.Id
            };
        }

        private async Task<Customer> CreateOrUpdateCustomerAsync(Session session, string description)
        {
            var customerService = new CustomerService();

            if (session.CustomerId.IsNullOrEmpty())
            {
                return await customerService.CreateAsync(new CustomerCreateOptions
                {
                    Description = description
                });
            }

            return await customerService.UpdateAsync(session.CustomerId, new CustomerUpdateOptions
            {
                Description = description
            });
        }

        public void HandleEvent(RecurringPaymentsDisabledEventData eventData)
        {
            var subscriptionPayment = _paymentManager.GetLastCompletedSubscriptionPayment(eventData.TenantId);

            var subscriptionService = new SubscriptionService();
            subscriptionService.Update(subscriptionPayment.ExternalPaymentId, new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                CollectionMethod = "send_invoice",
                DaysUntilDue = eventData.DaysUntilDue
            });
        }

        public void HandleEvent(RecurringPaymentsEnabledEventData eventData)
        {
            var subscriptionPayment = _paymentManager.GetLastCompletedSubscriptionPayment(eventData.TenantId);

            if (subscriptionPayment == null || subscriptionPayment.ExternalPaymentId.IsNullOrEmpty())
            {
                return;
            }

            var subscriptionService = new SubscriptionService();
            subscriptionService.Update(subscriptionPayment.ExternalPaymentId, new SubscriptionUpdateOptions
            {
                CancelAtPeriodEnd = false,
                CollectionMethod = "charge_automatically"
            });
        }

        public void HandleEvent(SubscriptionCancelledEventData eventData)
        {
            AsyncHelper.RunSync(() => CancelSubscription(eventData.ExternalPaymentId));
        }

        public void HandleEvent(SubscriptionUpdatedEventData eventData)
        {
            var subscriptionPayment = _subscriptionPaymentRepository.Get(eventData.PaymentId);

            if (subscriptionPayment == null || subscriptionPayment.ExternalPaymentId.IsNullOrEmpty())
            {
                // no subscription on stripe !
                return;
            }

            if (eventData.NewPlanId.IsNullOrEmpty() || (eventData.NewPlanAmount ?? 0) == 0)
            {
                AsyncHelper.RunSync(() => CancelSubscription(subscriptionPayment.ExternalPaymentId));
                return;
            }

            var payment = new SubscriptionPayment
            {
                TenantId = eventData.TenantId,
                PaymentPeriodType = subscriptionPayment.GetPaymentPeriodType(),
                Gateway = SubscriptionPaymentGatewayType.Stripe,
                IsRecurring = true,
                DayCount = subscriptionPayment.DayCount,
                SubscriptionPaymentProducts = new List<SubscriptionPaymentProduct>
                {
                    new(
                        description: eventData.Description,
                        amount: eventData.NewPlanAmount,
                        count: 1,
                        extraProperties: eventData.ExtraProperties
                    )
                }
            };

            _subscriptionPaymentRepository.InsertAndGetId(payment);

            CurrentUnitOfWork.SaveChanges();

            if (!AsyncHelper.RunSync(() => DoesPlanExistAsync(eventData.NewPlanId)))
            {
                AsyncHelper.RunSync(() =>
                    CreatePlanAsync(
                        eventData.NewPlanId,
                        subscriptionPayment.GetPlanType(),
                        eventData.NewPlanAmount.Value,
                        GetPlanInterval(subscriptionPayment.PaymentPeriodType),
                        AbpZeroTemplateConsts.ProductName
                    )
                );
            }

            AsyncHelper.RunSync(() => UpdateSubscription(subscriptionPayment.ExternalPaymentId, eventData.NewPlanId));
        }

        public async Task HandleInvoicePaymentSucceededAsync(Invoice invoice)
        {
            var customerService = new CustomerService();
            var customer = await customerService.GetAsync(invoice.CustomerId);

            int tenantId;
            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await _tenantManager.FindByTenancyNameAsync(customer.Description);
                tenantId = tenant.Id;
            }

            await EventBus.TriggerAsync(
                new RecurringPaymentSucceedEventData(tenantId, invoice.ChargeId, invoice.AmountPaid)
                {
                    Items = invoice.Lines.Select(i => new RecurringPaymentLineItem
                    {
                        Product = i.Plan.ProductId,
                        PlanId = i.Plan.Id,
                        PlanType = i.Plan.Metadata.GetValueOrDefault(PaymentConsts.PlanType),
                        Amount = ConvertFromStripePrice(i.Amount)
                    }).ToList()
                }
            );
        }

        public long ConvertToStripePrice(decimal amount)
        {
            return Convert.ToInt64(amount * 100);
        }

        public decimal ConvertFromStripePrice(long amount)
        {
            return Convert.ToDecimal(amount) / 100;
        }

        private async Task<StripeIdResponse> CreatePlanAsync(
            string planId,
            string planType,
            decimal amount,
            string interval,
            string productId)
        {
            var planService = new PlanService();
            var plan = await planService.CreateAsync(new PlanCreateOptions
            {
                Id = planId,
                Amount = ConvertToStripePrice(amount),
                Interval = interval,
                Product = productId,
                Currency = AbpZeroTemplateConsts.Currency,
                Metadata = new Dictionary<string, string>
                {
                    {PaymentConsts.PlanType, planType}
                }
            });

            return new StripeIdResponse
            {
                Id = plan.Id
            };
        }

        private async Task<StripeIdResponse> CreateProductAsync(string name)
        {
            var productService = new ProductService();
            var product = await productService.CreateAsync(new ProductCreateOptions
            {
                Id = name,
                Name = name,
                Type = "service"
            });

            return new StripeIdResponse
            {
                Id = product.Id
            };
        }

        public async Task<StripeIdResponse> GetOrCreatePlanForPayment(long paymentId)
        {
            var payment = await _subscriptionPaymentRepository.GetPaymentWithProducts(paymentId);
            var planId = payment.GetPlanId();
            var planType = payment.GetPlanType();

            var product = await GetOrCreateProductAsync(AbpZeroTemplateConsts.ProductName);
            var planInterval = GetPlanInterval(payment.PaymentPeriodType);

            return await GetOrCreatePlanAsync(
                planId,
                planType,
                payment.GetTotalAmount(),
                planInterval,
                product.Id
            );
        }
    }
}