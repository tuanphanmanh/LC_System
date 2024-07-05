using MyCompanyName.AbpZeroTemplate.Models.NavigationMenu;

namespace MyCompanyName.AbpZeroTemplate.Services.Navigation
{
    public interface IMenuProvider
    {
        List<NavigationMenuItem> GetAuthorizedMenuItems(Dictionary<string, string> grantedPermissions);
    }
}