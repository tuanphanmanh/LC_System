using Abp.Dependency;

namespace MyCompanyName.AbpZeroTemplate.DashboardCustomization.Definitions.Cache
{
    public interface IDashboardDefinitionCacheManager : ITransientDependency
    {
        DashboardDefinition Get(string name);

        void Set(DashboardDefinition definition);
    }
}