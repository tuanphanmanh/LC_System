using Abp.AutoMapper;
using MyCompanyName.AbpZeroTemplate.Authorization.Users.Dto;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(UserListDto))]
    public class UserListModel : UserListDto
    {
        public string Photo { get; set; }

        public string FullName => Name + " " + Surname;
    }
}
