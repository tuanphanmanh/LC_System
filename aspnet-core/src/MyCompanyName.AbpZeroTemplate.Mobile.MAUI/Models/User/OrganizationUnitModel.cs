using Abp.AutoMapper;
using MyCompanyName.AbpZeroTemplate.Organizations.Dto;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(OrganizationUnitDto))]
    public class OrganizationUnitModel : OrganizationUnitDto
    {
        public bool IsAssigned { get; set; }
    }
}
