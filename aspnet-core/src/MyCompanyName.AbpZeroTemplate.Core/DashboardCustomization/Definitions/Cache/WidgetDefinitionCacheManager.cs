using System.Collections.Generic;
using System.Linq;
using Abp.Runtime.Caching;

namespace MyCompanyName.AbpZeroTemplate.DashboardCustomization.Definitions.Cache
{
    public class WidgetDefinitionCacheManager : IWidgetDefinitionCacheManager
    {
        private const string WidgetDefinitionsCacheName = "WidgetDefinitionsCacheName";

        private readonly ITypedCache<string, List<WidgetDefinition>> _cache;

        public WidgetDefinitionCacheManager(ICacheManager cacheManager)
        {
            _cache = cacheManager.GetCache<string, List<WidgetDefinition>>(WidgetDefinitionsCacheName);
        }

        public WidgetDefinition Get(string name, string id)
        {
            var widgetDefinitions = _cache.GetOrDefault(name);
            return widgetDefinitions.FirstOrDefault(e => e.Id == id);
        }

        public void Set(string name, List<WidgetDefinition> definitions)
        {
            _cache.Set(name, definitions);
        }

        public List<WidgetDefinition> GetAll(string name)
        {
            return _cache.GetOrDefault(name);
        }
    }
}