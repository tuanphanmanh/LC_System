using Microsoft.Extensions.Configuration;
using System.Reflection;
using MyCompanyName.AbpZeroTemplate.Core;

namespace MyCompanyName.AbpZeroTemplate.Mobile.MAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            ApplicationBootstrapper.InitializeIfNeeds<AbpZeroTemplateMobileMAUIModule>();

            var app = builder.Build();
            return app;
        }
    }
}