using Microsoft.AspNetCore.Components;
using MyCompanyName.AbpZeroTemplate.Authorization.Accounts;
using MyCompanyName.AbpZeroTemplate.Authorization.Accounts.Dto;
using MyCompanyName.AbpZeroTemplate.Core.Dependency;
using MyCompanyName.AbpZeroTemplate.Core.Threading;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Models.Login;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Shared;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Pages.Login
{
    public partial class ForgotPasswordModal : ModalBase
    {
        public override string ModalId => "forgot-password-modal";
       
        [Parameter] public EventCallback OnSave { get; set; }
        
        public ForgotPasswordModel forgotPasswordModel { get; set; } = new ForgotPasswordModel();

        private readonly IAccountAppService _accountAppService;

        public ForgotPasswordModal()
        {
            _accountAppService = DependencyResolver.Resolve<IAccountAppService>();
        }

        protected virtual async Task Save()
        {
            await SetBusyAsync(async () =>
            {
                await WebRequestExecuter.Execute(
                async () =>
                    await _accountAppService.SendPasswordResetCode(new SendPasswordResetCodeInput { EmailAddress = forgotPasswordModel.EmailAddress }),
                    async () =>
                    {
                        await OnSave.InvokeAsync();
                    }
                );
            });
        }

        protected virtual async Task Cancel()
        {
            await Hide();
        }
    }
}
