using System;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto
{
    public class StartUpgradeSubscriptionOutput
    {
        public long? PaymentId { get; set; }

        public bool Upgraded { get; set; }

        public StartUpgradeSubscriptionOutput(bool upgraded, long? paymentId = null)
        {
            if (!upgraded && !paymentId.HasValue)
            {
                throw new ArgumentException("paymentId can not be null if upgraded is false!");
            }
            
            Upgraded = upgraded;
            PaymentId = paymentId;
        }
    }
}