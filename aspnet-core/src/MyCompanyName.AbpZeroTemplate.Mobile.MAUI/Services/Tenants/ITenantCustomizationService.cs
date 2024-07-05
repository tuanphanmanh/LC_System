namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Services.Tenants
{
    public interface ITenantCustomizationService
    {
        Task<string> GetTenantLogo();
    }
}