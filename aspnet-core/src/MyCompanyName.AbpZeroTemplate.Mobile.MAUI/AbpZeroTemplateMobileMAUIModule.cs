using Abp.AutoMapper;
using Abp.Configuration.Startup;
using Abp.Modules;
using Abp.Reflection.Extensions;
using MyCompanyName.AbpZeroTemplate.ApiClient;
using MyCompanyName.AbpZeroTemplate.Mobile.MAUI.Core.ApiClient;

namespace MyCompanyName.AbpZeroTemplate
{
    [DependsOn(typeof(AbpZeroTemplateClientModule), typeof(AbpAutoMapperModule))]

    public class AbpZeroTemplateMobileMAUIModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.IsEnabled = false;
            Configuration.BackgroundJobs.IsJobExecutionEnabled = false;

            Configuration.ReplaceService<IApplicationContext, MAUIApplicationContext>();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpZeroTemplateMobileMAUIModule).GetAssembly());
        }
    }
}