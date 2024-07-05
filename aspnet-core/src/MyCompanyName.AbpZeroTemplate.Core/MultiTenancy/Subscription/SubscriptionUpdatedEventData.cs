using Abp.Events.Bus;
using MyCompanyName.AbpZeroTemplate.ExtraProperties;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Subscription
{
    public class SubscriptionUpdatedEventData : EventData
    {
        public int TenantId { get; set; }
        
        public long PaymentId { get; set; }
        
        public string ExternalPaymentId { get; set; }
        
        public string NewPlanId { get; set; }
        
        public decimal? NewPlanAmount { get; set; }
        
        public string Description { get; set; }
        
        public ExtraPropertyDictionary ExtraProperties { get; set; }

        public SubscriptionUpdatedEventData()
        {
            ExtraProperties = new ExtraPropertyDictionary();
        }
    }
}