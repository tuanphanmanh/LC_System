using Abp.AutoMapper;
using MyCompanyName.AbpZeroTemplate.Authorization.Users.Dto;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Models.User
{
    [AutoMapFrom(typeof(CreateOrUpdateUserInput))]
    public class UserCreateOrUpdateModel : CreateOrUpdateUserInput
    {

    }
}
