using System.Net;
using Abp.Web.Models;
using Castle.Core.Internal;
using Flurl.Http;
using MyCompanyName.AbpZeroTemplate.ApiClient;
using MyCompanyName.AbpZeroTemplate.Core.Dependency;
using MyCompanyName.AbpZeroTemplate.Extensions;
using MyCompanyName.AbpZeroTemplate.Localization;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Services.UI;

namespace MyCompanyName.AbpZeroTemplate.Core.Threading
{
    public class AbpExceptionHandler
    {
        public async Task<bool> HandleIfAbpResponseAsync(FlurlHttpException httpException)
        {
            AjaxResponse ajaxResponse = await httpException.GetResponseJsonAsync<AjaxResponse>();
            if (ajaxResponse == null)
            {
                return false;
            }

            if (!ajaxResponse.__abp)
            {
                return false;
            }

            if (ajaxResponse.Error == null)
            {
                return false;
            }

            if (IsUnauthroizedResponseForSessionTimoutCase(httpException, ajaxResponse))
            {
                return true;
            }

            await UserDialogsService.Instance.UnBlock();

            if (string.IsNullOrEmpty(ajaxResponse.Error.Details))
            {
                await UserDialogsService.Instance.AlertError(ajaxResponse.Error.GetConsolidatedMessage());
            }
            else
            {
                await UserDialogsService.Instance.AlertError(ajaxResponse.Error.Details, ajaxResponse.Error.GetConsolidatedMessage());
            }

            return true;
        }

        /// <summary>
        /// AuthenticationHttpHandler handles unauthorized responses and reauthenticates if there's a valid refresh token.
        /// When the refresh token expires, the application logsout and forces user to re-enter credentials
        /// That's why the last unauthorized exception can be suspended.
        /// </summary>
        private static bool IsUnauthroizedResponseForSessionTimoutCase(FlurlHttpException httpException, AjaxResponse ajaxResponse)
        {
            if (httpException.Call.HttpResponseMessage.StatusCode != HttpStatusCode.Unauthorized)
            {
                return false;
            }

            var accessTokenManager = DependencyResolver.Resolve<IAccessTokenManager>();

            var errorMsg = L.Localize("CurrentUserDidNotLoginToTheApplication", "Abp");

            if (accessTokenManager.IsUserLoggedIn)
            {
                return false;
            }

            if (!ajaxResponse.Error.Message.EqualsText(errorMsg))
            {
                return false;
            }

            return true;
        }
    }
}