using System.Linq;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Paypal;

namespace MyCompanyName.AbpZeroTemplate.Web.Models.Paypal
{
    public class PayPalPurchaseViewModel
    {
        public SubscriptionPaymentDto Payment { get; set; }

        public decimal Amount { get; set; }

        public PayPalPaymentGatewayConfiguration Configuration { get; set; }

        public string GetDisabledFundingsQueryString()
        {
            if (Configuration.DisabledFundings == null || !Configuration.DisabledFundings.Any())
            {
                return "";
            }

            return "&disable-funding=" + string.Join(',', Configuration.DisabledFundings.ToList());
        }
    }
}
