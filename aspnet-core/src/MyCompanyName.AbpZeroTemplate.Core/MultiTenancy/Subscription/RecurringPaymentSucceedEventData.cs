using System.Collections.Generic;
using Abp.Events.Bus;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription
{
    public class RecurringPaymentSucceedEventData : EventData
    {
        public int TenantId { get; set; }

        public string ExternalPaymentId { get; set; }

        public decimal TotalAmount { get; set; }

        public List<RecurringPaymentLineItem> Items { get; set; }

        public RecurringPaymentSucceedEventData(int tenantId, string externalPaymentId, decimal totalAmount)
        {
            TenantId = tenantId;
            ExternalPaymentId = externalPaymentId;
            TotalAmount = totalAmount;
            Items = new List<RecurringPaymentLineItem>();
        }
    }

    public class RecurringPaymentLineItem
    {
        public string Product { get; set; }

        public string PlanId { get; set; }
        
        public string PlanType { get; set; }
        
        public decimal Amount { get; set; }
    }
}