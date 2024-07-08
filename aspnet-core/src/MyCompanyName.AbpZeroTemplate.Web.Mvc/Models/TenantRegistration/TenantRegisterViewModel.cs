using MyCompanyName.AbpZeroTemplate.Editions;
using MyCompanyName.AbpZeroTemplate.Editions.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments;
using MyCompanyName.AbpZeroTemplate.Security;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;

namespace MyCompanyName.AbpZeroTemplate.Web.Models.TenantRegistration
{
    public class TenantRegisterViewModel
    {
        public int? EditionId { get; set; }

        public EditionSelectDto Edition { get; set; }
        
        public PasswordComplexitySetting PasswordComplexitySetting { get; set; }

        public EditionPaymentType EditionPaymentType { get; set; }
        
        public SubscriptionStartType? SubscriptionStartType { get; set; }
        
        public PaymentPeriodType? PaymentPeriodType { get; set; }
        
        public string SuccessUrl { get; set; }
        
        public string ErrorUrl { get; set; }
    }
}
