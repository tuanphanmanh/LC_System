using MyCompanyName.AbpZeroTemplate.MultiTenancy.Dto;
using MyCompanyName.AbpZeroTemplate.Sessions.Dto;

namespace MyCompanyName.AbpZeroTemplate.Web.Areas.AppAreaName.Models.Editions
{
    public class SubscriptionDashboardViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; set; }
        
        public EditionsSelectOutput Editions { get; set; }
    }
}
