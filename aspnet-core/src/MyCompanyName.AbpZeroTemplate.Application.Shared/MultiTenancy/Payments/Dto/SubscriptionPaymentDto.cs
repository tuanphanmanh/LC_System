using System.Collections.Generic;
using System.Linq;
using Abp.Application.Services.Dto;
using  MyCompanyName.AbpZeroTemplate.ExtraProperties;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto
{
    public class SubscriptionPaymentDto : EntityDto<long>
    {
        public SubscriptionPaymentGatewayType Gateway { get; set; }
        
        public int TenantId { get; set; }

        public int DayCount { get; set; }

        public PaymentPeriodType PaymentPeriodType { get; set; }

        public string PaymentId { get; set; }
        
        public long InvoiceNo { get; set; }

        public SubscriptionPaymentStatus Status { get; set; }

        public bool? IsRecurring { get; set; }
        
        public bool IsProrationPayment { get; set; }
        
        public string ExternalPaymentId { get; set; }

        public string SuccessUrl { get; set; }

        public string ErrorUrl { get; set; }

        public List<SubscriptionPaymentProductDto> SubscriptionPaymentProducts { get; set; }
        
        public ExtraPropertyDictionary ExtraProperties { get; set; }
        
        public SubscriptionPaymentDto()
        {
            SubscriptionPaymentProducts = new List<SubscriptionPaymentProductDto>();
            ExtraProperties = new ExtraPropertyDictionary();
        }
        
        public decimal TotalAmount { get; set; }
        
        public bool AllowRecurringPayment()
        {
            return !IsRecurring.HasValue;
        }
        
        public bool RecurringPaymentEnabled()
        {
            return IsRecurring.HasValue && IsRecurring.Value;
        }
    }
}
