using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Extensions;
using Abp.UI;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe.Dto;
using Stripe.Checkout;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments
{
    public class StripePaymentAppService : AbpZeroTemplateAppServiceBase, IStripePaymentAppService
    {
        private readonly ISubscriptionPaymentRepository _subscriptionPaymentRepository;
        private readonly StripeGatewayManager _stripeGatewayManager;
        private readonly StripePaymentGatewayConfiguration _stripePaymentGatewayConfiguration;
        
        public StripePaymentAppService(
            StripeGatewayManager stripeGatewayManager,
            StripePaymentGatewayConfiguration stripePaymentGatewayConfiguration,
            ISubscriptionPaymentRepository subscriptionPaymentRepository)
        {
            _stripeGatewayManager = stripeGatewayManager;
            _stripePaymentGatewayConfiguration = stripePaymentGatewayConfiguration;
            _subscriptionPaymentRepository = subscriptionPaymentRepository;
        }

        [RemoteService(false)]
        public async Task ConfirmPayment(StripeConfirmPaymentInput input)
        {
            var payment = await _subscriptionPaymentRepository
                .GetPaymentWithProducts(input.PaymentId);

            var sessionId =
                payment.ExtraProperties[StripeGatewayManager.StripeSessionIdSubscriptionPaymentExtensionDataKey] as
                    string;

            if (sessionId.IsNullOrEmpty())
            {
                throw new ApplicationException(
                    $"Could not find StripeSessionId in SubscriptionPayment.ExtraProperties for PaymentId {input.PaymentId}");
            }

            if (payment.Status != SubscriptionPaymentStatus.NotPaid)
            {
                throw new ApplicationException(
                    $"Invalid payment status {payment.Status}, cannot create a charge on stripe !"
                );
            }

            payment.Gateway = SubscriptionPaymentGatewayType.Stripe;

            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            if (session.Mode == "payment")
            {
                payment.ExternalPaymentId = session.PaymentIntentId;
            }
            else if (session.Mode == "subscription")
            {
                payment.ExternalPaymentId = session.SubscriptionId;
            }
            else
            {
                throw new ApplicationException(
                    $"Unexpected session mode {session.Mode}. 'payment' or 'subscription' expected");
            }

            payment.SetAsPaid();

            var tenant = await TenantManager.GetByIdAsync(payment.TenantId);
            if (tenant != null && payment.RecurringPaymentEnabled())
            {
                tenant.SubscriptionPaymentType = SubscriptionPaymentType.RecurringAutomatic;
                await TenantManager.UpdateAsync(tenant);
            }
            
            await CurrentUnitOfWork.SaveChangesAsync();

            if (payment.IsProrationPayment)
            {
                await ConfirmSubscriptionUpgradeProrationPayment(payment);
            }
        }

        private async Task ConfirmSubscriptionUpgradeProrationPayment(SubscriptionPayment payment)
        {
            await _stripeGatewayManager.UpdateSubscription(payment, true);
        }

        public StripeConfigurationDto GetConfiguration()
        {
            return new StripeConfigurationDto
            {
                PublishableKey = _stripePaymentGatewayConfiguration.PublishableKey
            };
        }

        public async Task<string> CreatePaymentSession(StripeCreatePaymentSessionInput input)
        {
            var payment = await _subscriptionPaymentRepository.GetPaymentWithProducts(input.PaymentId);

            var paymentTypes = _stripePaymentGatewayConfiguration.PaymentMethodTypes;
            var sessionCreateOptions = new SessionCreateOptions
            {
                PaymentMethodTypes = paymentTypes,
                SuccessUrl = input.SuccessUrl + (input.SuccessUrl.Contains("?") ? "&" : "?") +
                             $"paymentId={input.PaymentId.ToString()}",
                CancelUrl = input.CancelUrl,
                Metadata = new Dictionary<string, string>
                {
                    { "PaymentId", input.PaymentId.ToString() }
                }
            };

            if (payment.RecurringPaymentEnabled() && !payment.IsProrationPayment)
            {
                var plan = await _stripeGatewayManager.GetOrCreatePlanForPayment(input.PaymentId);

                sessionCreateOptions.Mode = "subscription";
                sessionCreateOptions.LineItems = new List<SessionLineItemOptions>
                {
                    new()
                    {
                        Price = plan.Id,
                        Quantity = 1
                    }
                };
            }
            else
            {
                sessionCreateOptions.Mode = "payment";
                sessionCreateOptions.LineItems = payment.SubscriptionPaymentProducts
                    .Select(p => new SessionLineItemOptions
                    {
                        Quantity = p.Count,
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = _stripeGatewayManager.ConvertToStripePrice(p.Amount),
                            Currency = AbpZeroTemplateConsts.Currency,
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Description = p.Description,
                                Name = AbpZeroTemplateConsts.ProductName
                            }
                        }
                    }).ToList();
            }

            var service = new SessionService();
            var session = await service.CreateAsync(sessionCreateOptions);

            payment.SetProperty(StripeGatewayManager.StripeSessionIdSubscriptionPaymentExtensionDataKey, session.Id);
            await _subscriptionPaymentRepository.UpdateAsync(payment);
            return session.Id;
        }

        public async Task<StripePaymentResultOutput> GetPaymentResult(StripePaymentResultInput input)
        {
            var payment = await _subscriptionPaymentRepository
                .FirstOrDefaultAsync(x => x.Id == input.PaymentId);

            if (payment is null)
            {
                throw new ApplicationException(
                    $"Could not find payment with PaymentId {input.PaymentId}");
            }

            var sessionId =
                payment.GetProperty<string>(StripeGatewayManager.StripeSessionIdSubscriptionPaymentExtensionDataKey);

            if (string.IsNullOrEmpty(sessionId))
            {
                throw new UserFriendlyException(L("ThereIsNoStripeSessionIdOnPayment", input.PaymentId));
            }

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await TenantManager.GetByIdAsync(payment.TenantId);
                await _stripeGatewayManager.UpdateCustomerDescriptionAsync(sessionId, tenant.TenancyName);
            }

            if (payment.Status == SubscriptionPaymentStatus.Paid)
            {
                return new StripePaymentResultOutput
                {
                    PaymentDone = true,
                    CallbackUrl = payment.SuccessUrl
                };
            }

            return new StripePaymentResultOutput
            {
                PaymentDone = false,
                CallbackUrl = payment.ErrorUrl
            };
        }
    }
}