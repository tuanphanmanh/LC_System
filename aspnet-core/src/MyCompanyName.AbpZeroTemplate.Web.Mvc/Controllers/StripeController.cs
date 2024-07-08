using System;
using System.Threading.Tasks;
using Abp.Extensions;
using Microsoft.AspNetCore.Mvc;
using MyCompanyName.AbpZeroTemplate.MultiTenancy;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe.Dto;
using MyCompanyName.AbpZeroTemplate.Url;
using MyCompanyName.AbpZeroTemplate.Web.Models.Stripe;

namespace MyCompanyName.AbpZeroTemplate.Web.Controllers
{
    public class StripeController : StripeControllerBase
    {
        private readonly StripePaymentGatewayConfiguration _stripeConfiguration;
        private readonly IPaymentAppService _paymentAppService;
        private readonly IWebUrlService _webUrlService;
        private readonly StripeGatewayManager _stripeGatewayManager;
        private readonly TenantManager _tenantManager;

        public StripeController(
            StripeGatewayManager stripeGatewayManager,
            StripePaymentGatewayConfiguration stripeConfiguration,
            IStripePaymentAppService stripePaymentAppService,
            IPaymentAppService paymentAppService,
            IWebUrlService webUrlService, TenantManager tenantManager)
            : base(stripeGatewayManager, stripeConfiguration, stripePaymentAppService)
        {
            _stripeGatewayManager = stripeGatewayManager;
            _stripeConfiguration = stripeConfiguration;
            _paymentAppService = paymentAppService;
            _webUrlService = webUrlService;
            _tenantManager = tenantManager;
        }

        public async Task<ActionResult> PrePayment(long paymentId)
        {
            var payment = await _paymentAppService.GetPaymentAsync(paymentId);
            SetTenantIdCookie(payment.TenantId);

            if (payment.Status != SubscriptionPaymentStatus.NotPaid)
            {
                throw new ApplicationException("This payment is processed before");
            }

            var model = new StripePurchaseViewModel
            {
                Payment = payment,
                Amount = payment.TotalAmount,
                IsRecurring = payment.RecurringPaymentEnabled(),
                IsProrationPayment = payment.IsProrationPayment,
                Configuration = _stripeConfiguration,
            };

            var sessionId = await StripePaymentAppService.CreatePaymentSession(new StripeCreatePaymentSessionInput
            {
                PaymentId = paymentId,
                SuccessUrl = _webUrlService.GetSiteRootAddress().EnsureEndsWith('/') + "Stripe/PostPayment",
                CancelUrl = _webUrlService.GetSiteRootAddress().EnsureEndsWith('/') + "Stripe/CancelPayment",
            });

            model.SessionId = sessionId;

            return View("~/Views/Payment/Stripe/PrePayment.cshtml", model);
        }

        public async Task<ActionResult> PostPayment(long paymentId)
        {
            var payment = await _paymentAppService.GetPaymentAsync(paymentId);

            var sessionId =
                payment.ExtraProperties[StripeGatewayManager.StripeSessionIdSubscriptionPaymentExtensionDataKey] as
                    string;

            using (CurrentUnitOfWork.SetTenantId(null))
            {
                var tenant = await _tenantManager.GetByIdAsync(payment.TenantId);
                await _stripeGatewayManager.UpdateCustomerDescriptionAsync(sessionId, tenant.TenancyName);
            }

            ViewBag.PaymentId = payment.Id;
            return View("~/Views/Payment/Stripe/PostPayment.cshtml");
        }
        
        public IActionResult CancelPayment()
        {
            return View("~/Views/Payment/Stripe/CancelPayment.cshtml");
        }
    }
}