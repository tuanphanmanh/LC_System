using System.Collections.Generic;
using Abp.Dependency;

namespace MyCompanyName.AbpZeroTemplate.DashboardCustomization.Definitions.Cache
{
    public interface IWidgetDefinitionCacheManager: ITransientDependency
    {
        /// <summary>
        /// Gets a single WidgetDefinition
        /// </summary>
        /// <param name="name">name of cache item which holds list of WidgetDefinitions</param>
        /// <param name="id">Id of the specific widget</param>
        /// <returns></returns>
        WidgetDefinition Get(string name, string id);

        /// <summary>
        /// Sets a group of widgets with a unique name
        /// </summary>
        /// <param name="name">name of cache item which holds list of WidgetDefinitions</param>
        /// <param name="definitions">List of widget definitions</param>
        void Set(string name,  List<WidgetDefinition> definitions);
        
        /// <summary>
        /// Gets all widgets in a widget cache
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        List<WidgetDefinition> GetAll(string name);
    }
}