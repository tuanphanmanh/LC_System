using System.Threading.Tasks;
using Abp.Application.Services;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Dto;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Payments.Dto;

namespace MyCompanyName.AbpZeroTemplate.MultiTenancy
{
    public interface ISubscriptionAppService : IApplicationService
    {
        Task DisableRecurringPayments();

        Task EnableRecurringPayments();
        
        Task<long> StartExtendSubscription(StartExtendSubscriptionInput input);
        
        Task<StartUpgradeSubscriptionOutput> StartUpgradeSubscription(StartUpgradeSubscriptionInput input);
        
        Task<long> StartTrialToBuySubscription(StartTrialToBuySubscriptionInput input);
    }
}
