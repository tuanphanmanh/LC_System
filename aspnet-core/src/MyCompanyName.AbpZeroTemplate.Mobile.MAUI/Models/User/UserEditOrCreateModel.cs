using Abp.AutoMapper;
using MyCompanyName.AbpZeroTemplate.Authorization.Users.Dto;


namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(GetUserForEditOutput))]
    public class UserEditOrCreateModel : GetUserForEditOutput
    {
        public string Photo { get; set; }

        public string FullName => User == null ? string.Empty : User.Name + " " + User.Surname;

        public DateTime CreationTime { get; set; }

        public bool IsEmailConfirmed { get; set; }

        private List<OrganizationUnitModel> _organizationUnits;
        public List<OrganizationUnitModel> OrganizationUnits
        {
            get => _organizationUnits;
            set
            {
                _organizationUnits = value?.OrderBy(o => o.Code).ToList();
                SetAsAssignedForMemberedOrganizationUnits();
            }
        }

        private void SetAsAssignedForMemberedOrganizationUnits()
        {
            if (_organizationUnits != null)
            {
                MemberedOrganizationUnits?.ForEach(memberedOrgUnitCode =>
                {
                    _organizationUnits
                        .Single(o => o.Code == memberedOrgUnitCode)
                        .IsAssigned = true;
                });
            }
        }       
    }
}
