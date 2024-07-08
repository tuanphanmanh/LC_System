using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MyCompanyName.AbpZeroTemplate.Web.Areas.AppAreaName.Models.Layout;
using MyCompanyName.AbpZeroTemplate.Web.Views;

namespace MyCompanyName.AbpZeroTemplate.Web.Areas.AppAreaName.Views.Shared.Components.AppAreaNameToggleDarkMode
{
    public class AppAreaNameToggleDarkModeViewComponent : AbpZeroTemplateViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(string cssClass, bool isDarkModeActive)
        {
            return Task.FromResult<IViewComponentResult>(View(new ToggleDarkModeViewModel(cssClass, isDarkModeActive)));
        }
    }
}