using Abp.AutoMapper;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Dto;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Models.Tenants
{
    [AutoMapFrom(typeof(TenantListDto))]
    [AutoMapTo(typeof(TenantEditDto), typeof(CreateTenantInput))]
    public class TenantListModel : TenantListDto
    {
 
    }
}
