using MyCompanyName.AbpZeroTemplate.ExtraProperties;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto
{
    public class CreatePaymentProductDto : IHasExtraProperties
    {
        public string Description { get; set; }
    
        public decimal Amount { get; set; }
    
        public int Count { get; set; }
        
        public ExtraPropertyDictionary ExtraProperties { get; set; }
    }
}