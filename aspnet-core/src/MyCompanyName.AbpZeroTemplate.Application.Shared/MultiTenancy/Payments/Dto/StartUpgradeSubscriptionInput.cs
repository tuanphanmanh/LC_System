namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto
{
    public class StartUpgradeSubscriptionInput
    {
        public int TargetEditionId { get; set; }
        
        public string SuccessUrl { get; set; }
        
        public string ErrorUrl { get; set; }
        
        public PaymentPeriodType? PaymentPeriodType { get; set; }
    }
}