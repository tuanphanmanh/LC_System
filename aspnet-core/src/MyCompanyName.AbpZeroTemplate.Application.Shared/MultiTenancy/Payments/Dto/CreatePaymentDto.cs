using System.Collections.Generic;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto
{
    public class CreatePaymentDto : IHasExtraProperties
    {
        public int TenantId { get; set; }
        
        public PaymentPeriodType? PaymentPeriodType { get; set; }

        public SubscriptionPaymentGatewayType SubscriptionPaymentGatewayType { get; set; }

        public bool? RecurringPaymentEnabled { get; set; }
        
        public bool? IsProrationPayment { get; set; }

        public string SuccessUrl { get; set; }

        public string ErrorUrl { get; set; }
        
        public ExtraPropertyDictionary ExtraProperties { get; set; }
        
        public List<CreatePaymentProductDto> Products { get; set; }
    }
    
    public class UpdatePaymentDto : IHasExtraProperties
    {
        public long PaymentId { get; set; }
        
        public string Gateway { get; set; }
        
        public bool IsRecurring { get; set; }
        
        public ExtraPropertyDictionary ExtraProperties { get; set; }
    }
}
