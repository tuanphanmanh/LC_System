using Microsoft.AspNetCore.Components;
using MyCompanyName.AbpZeroTemplate.Authorization.Users.Profile;
using MyCompanyName.AbpZeroTemplate.Authorization.Users.Profile.Dto;
using MyCompanyName.AbpZeroTemplate.Core.Dependency;
using MyCompanyName.AbpZeroTemplate.Core.Threading;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Models.Settings;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Shared;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Pages.MySettings
{
    public partial class ChangePasswordModal : ModalBase
    {
        [Parameter] public EventCallback OnSave { get; set; }

        public override string ModalId => "change-password";

        public ChangePasswordModel ChangePasswordModel { get; set; } = new ChangePasswordModel();

        private readonly IProfileAppService _profileAppService;

        public ChangePasswordModal()
        {
            _profileAppService = DependencyResolver.Resolve<IProfileAppService>();
        }

        protected virtual async Task Save()
        {
            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(async () =>
                {
                    await _profileAppService.ChangePassword(new ChangePasswordInput
                    {
                        CurrentPassword = ChangePasswordModel.CurrentPassword,
                        NewPassword = ChangePasswordModel.NewPassword
                    });

                }, async () =>
                {
                    if (ChangePasswordModel.IsChangePasswordDisabled)
                    {
                        return;
                    }

                    await OnSave.InvokeAsync();
                });
            });
        }

        protected virtual async Task Cancel()
        {
            await Hide();
        }
    }
}
