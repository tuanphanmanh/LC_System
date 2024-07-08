using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Stripe;

namespace MyCompanyName.AbpZeroTemplate.Web.Models.Stripe
{
    public class StripePurchaseViewModel
    {
        public SubscriptionPaymentDto Payment { get; set; }
        
        public decimal Amount { get; set; }

        public bool IsRecurring { get; set; }
        
        public bool IsProrationPayment { get; set; }

        public string SessionId { get; set; }

        public StripePaymentGatewayConfiguration Configuration { get; set; }
    }
}
