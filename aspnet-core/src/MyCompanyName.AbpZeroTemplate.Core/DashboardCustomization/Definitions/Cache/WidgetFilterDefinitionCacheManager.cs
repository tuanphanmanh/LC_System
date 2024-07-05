using System.Collections.Generic;
using System.Linq;
using Abp.Runtime.Caching;

namespace MyCompanyName.AbpZeroTemplate.DashboardCustomization.Definitions.Cache
{
    public class WidgetFilterDefinitionCacheManager : IWidgetFilterDefinitionCacheManager
    {
        private const string WidgetFilterDefinitionCacheName = "WidgetFilterDefinitionCacheName";
        private const string WidgetFilterDefinitionCacheItemName = "WidgetFilterDefinitionCacheItemName";
        
        private readonly ITypedCache<string, List<WidgetFilterDefinition>> _cache;

        public WidgetFilterDefinitionCacheManager(ICacheManager cacheManager)
        {
            _cache = cacheManager.GetCache<string, List<WidgetFilterDefinition>>(WidgetFilterDefinitionCacheName);
        }

        public List<WidgetFilterDefinition> GetAll()
        {
            return _cache.GetOrDefault(WidgetFilterDefinitionCacheItemName);
        }
        
        public void Set(List<WidgetFilterDefinition> definition)
        {
            _cache.Set(WidgetFilterDefinitionCacheItemName, definition);
        }
    }
}