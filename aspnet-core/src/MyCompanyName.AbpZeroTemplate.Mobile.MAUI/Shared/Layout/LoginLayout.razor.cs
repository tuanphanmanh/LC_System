using Abp.Threading;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MyCompanyName.AbpZeroTemplate.Core.Dependency;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Core.ApiClient;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Services.Tenants;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Services.UI;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Shared.Layout
{
    public partial class LoginLayout
    {
        [Inject]
        protected IJSRuntime JS { get; set; }

        private string _logoURL;

        protected override async Task OnInitializedAsync()
        {
            MAUIApplicationContext.OnTenantChange += OnTenantChange;
            await SetLogoUrl();
        }

        private void OnTenantChange(object sender, EventArgs e)
        {
            AsyncHelper.RunSync(SetLogoUrl);
            StateHasChanged();
        }

        private async Task SetLogoUrl()
        {
            _logoURL = await DependencyResolver.Resolve<TenantCustomizationService>().GetTenantLogo();
        }

        private async Task SetLayout()
        {
            var dom = DependencyResolver.Resolve<DomManipulatorService>();
            await dom.ClearAllAttributes(JS, "body");
            await dom.SetAttribute(JS, "body", "id", "kt_body");
            await dom.SetAttribute(JS, "body", "class", "app-blank");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {

            if (firstRender)
            {
                await Task.Delay(200);
                await SetLayout();
                await JS.InvokeVoidAsync("KTMenu.init");
            }
        }
    }
}
