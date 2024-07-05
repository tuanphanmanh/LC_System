using Abp.Application.Services.Dto;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using MyCompanyName.AbpZeroTemplate.Core.Dependency;
using MyCompanyName.AbpZeroTemplate.Core.Threading;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Shared;
using MyCompanyName.AbpZeroTemplate.MultiTenancy;
using MyCompanyName.AbpZeroTemplate.MultiTenancy.Dto;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Pages.Tenant
{
    public partial class Index : AbpZeroTemplateMainLayoutPageComponentBase
    {
        protected ITenantAppService tenantAppService { get; set; }

        private ItemsProviderResult<TenantListDto> tenants;

        private TenantListDto editTenant;

        private EditTenantModal editTenantModal { get; set; }

        private CreateTenantModal createTenantModal { get; set; }

        private GetTenantsInput _filter = new GetTenantsInput();

        private Virtualize<TenantListDto> TenantListContainer { get; set; }

        public Index()
        {
            tenantAppService = DependencyResolver.Resolve<ITenantAppService>();
        }

        private async Task OpenCreateModal()
        {
            await createTenantModal.Open();
        }

        protected override async Task OnInitializedAsync()
        {
            await SetPageHeader(L("Tenants"), new List<Services.UI.PageHeaderButton>()
            {
                new Services.UI.PageHeaderButton(L("CreateNewTenant"), OpenCreateModal)
            });
        }

        private async Task EditTenant(TenantListDto tenant)
        {
            editTenant = tenant;
            await editTenantModal.OpenFor(tenant);
        }

        private async Task RefreshList()
        {
            await TenantListContainer.RefreshDataAsync();
            StateHasChanged();
        }

        private async ValueTask<ItemsProviderResult<TenantListDto>> LoadTenants(ItemsProviderRequest request)
        {
            _filter.MaxResultCount = request.Count;
            _filter.SkipCount = request.StartIndex;

            await UserDialogsService.Block();

            await WebRequestExecuter.Execute(
                 async () => await tenantAppService.GetTenants(_filter),
                 async (result) =>
                 {
                     tenants = new ItemsProviderResult<TenantListDto>(result.Items, result.TotalCount);
                     await UserDialogsService.UnBlock();
                 }
             );

            return tenants;
        }

        private async Task DeleteTenant(TenantListDto tenant)
        {
            var isConfirmed = await UserDialogsService.Confirm(L("TenantDeleteWarningMessage", "\"" + tenant.TenancyName + "\""), L("AreYouSure"));
            if (isConfirmed)
            {
                await SetBusyAsync(async () =>
                {
                    await RefreshList();
                });
            }
        }
    }
}
