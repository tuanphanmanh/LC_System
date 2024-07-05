using Abp.Runtime.Caching;

namespace MyCompanyName.AbpZeroTemplate.DashboardCustomization.Definitions.Cache
{
    public class DashboardDefinitionCacheManager : IDashboardDefinitionCacheManager
    {
        private const string DashboardDefinitionsCacheName = "DashboardDefinitionsCacheName";

        private readonly ITypedCache<string, DashboardDefinition> _cache;

        public DashboardDefinitionCacheManager(ICacheManager cacheManager)
        {
            _cache = cacheManager.GetCache<string, DashboardDefinition>(DashboardDefinitionsCacheName);
        }

        public DashboardDefinition Get(string name)
        {
            return _cache.GetOrDefault(name);
        }

        public void Set(DashboardDefinition definition)
        {
            _cache.Set(definition.Name, definition);
        }
    }
}