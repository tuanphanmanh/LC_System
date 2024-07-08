using MyCompanyName.AbpZeroTemplate.Sessions.Dto;

namespace MyCompanyName.AbpZeroTemplate.Web.Views.Shared.Components.AccountLogo
{
    public class AccountLogoViewModel
    {
        public GetCurrentLoginInformationsOutput LoginInformations { get; }
        
        public AccountLogoViewModel(GetCurrentLoginInformationsOutput loginInformations, string skin)
        {
            LoginInformations = loginInformations;
        }

        public string GetLogoUrl(string appPath, string skin)
        {
            if (LoginInformations?.Tenant == null || !LoginInformations.Tenant.HasLogo())
            {
                return appPath + "Common/Images/app-logo-on-" + skin + ".svg?";
            }

            return appPath + "TenantCustomization/GetTenantLogo?tenantId=" + LoginInformations?.Tenant?.Id + "&skin=" + skin;
        }
    }
}