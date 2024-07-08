using System.Collections.Generic;
using System.Linq;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;

namespace MyCompanyName.AbpZeroTemplate.Web.Models.Payment
{
    public class GatewaySelectionViewModel
    {
        public SubscriptionPaymentDto Payment { get; set; }
        
        public List<PaymentGatewayModel> PaymentGateways { get; set; }

        public bool AllowRecurringPaymentOption()
        {
            return Payment.AllowRecurringPayment() && PaymentGateways.Any(gateway => gateway.SupportsRecurringPayments);
        }
    }
}
