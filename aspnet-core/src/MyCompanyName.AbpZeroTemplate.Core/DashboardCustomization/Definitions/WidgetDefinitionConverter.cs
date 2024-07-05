using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Extensions;
using Abp.Json;
using Abp.MultiTenancy;
using System.Text.Json.Serialization;

namespace MyCompanyName.AbpZeroTemplate.DashboardCustomization.Definitions
{
    public class WidgetDefinitionConverter : JsonConverter<WidgetDefinition>
    {
        public override WidgetDefinition Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var jsonElement = JsonDocument.ParseValue(ref reader).RootElement;

            string id = null;
            if (jsonElement.TryGetProperty(nameof(WidgetDefinition.Id), out var idProperty))
            {
                id = idProperty.GetString();
            }

            string name = null;
            if (jsonElement.TryGetProperty(nameof(WidgetDefinition.Name), out var nameProperty))
            {
                name = nameProperty.GetString();
            }

            var side = MultiTenancySides.Tenant | MultiTenancySides.Host;
            if (jsonElement.TryGetProperty(nameof(WidgetDefinition.Side), out var sideProperty))
            {
                side = sideProperty.GetRawText().ToEnum<MultiTenancySides>();
            }

            List<string> usedWidgetFilters = null;
            if (jsonElement.TryGetProperty(nameof(WidgetDefinition.UsedWidgetFilters), out var usedWidgetFiltersProperty))
            {
                var usedWidgetFiltersString = usedWidgetFiltersProperty.GetString();
                usedWidgetFilters = string.IsNullOrEmpty(usedWidgetFiltersString)
                    ? null
                    : JsonSerializationHelper.DeserializeWithType<List<string>>(usedWidgetFiltersString);
            }

            string description = null;
            if (jsonElement.TryGetProperty(nameof(WidgetDefinition.Description), out var descriptionProperty))
            {
                description = descriptionProperty.GetString();
            }

            var allowMultipleInstanceInSamePage = false;
            if (jsonElement.TryGetProperty(nameof(WidgetDefinition.AllowMultipleInstanceInSamePage), out var allowMultipleInstanceInSamePageProperty))
            {
                allowMultipleInstanceInSamePage = allowMultipleInstanceInSamePageProperty.GetBoolean();
            }

            IPermissionDependency permissionDependency = null;
            if (jsonElement.TryGetProperty(nameof(WidgetDefinition.PermissionDependency), out var permissionDependencyProperty))
            {
                var permissionDependencyString = permissionDependencyProperty.GetString();
                permissionDependency = string.IsNullOrEmpty(permissionDependencyString)
                    ? null
                    : JsonSerializationHelper.DeserializeWithType<IPermissionDependency>(permissionDependencyString);
            }

            IFeatureDependency featureDependency = null;
            if (jsonElement.TryGetProperty(nameof(WidgetDefinition.FeatureDependency), out var featureDependencyProperty))
            {
                var featureDependencyString = featureDependencyProperty.GetString();
                featureDependency = string.IsNullOrEmpty(featureDependencyString)
                    ? null
                    : JsonSerializationHelper.DeserializeWithType<IFeatureDependency>(featureDependencyString);
            }

            var widgetDefinition = new WidgetDefinition(
                id,
                name,
                side,
                usedWidgetFilters,
                permissionDependency,
                description,
                allowMultipleInstanceInSamePage,
                featureDependency
            );
            return widgetDefinition;
        }

        public override void Write(Utf8JsonWriter writer, WidgetDefinition value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                return;
            }

            writer.WriteStartObject();

            writer.WritePropertyName(nameof(WidgetDefinition.Id));
            writer.WriteStringValue(value.Id);


            writer.WritePropertyName(nameof(WidgetDefinition.Name));
            writer.WriteStringValue(value.Name);

            writer.WritePropertyName(nameof(WidgetDefinition.Side));
            writer.WriteNumberValue(value.Side.To<int>());

            if (value.PermissionDependency != null)
            {
                writer.WritePropertyName(nameof(WidgetDefinition.PermissionDependency));

                var content = JsonSerializationHelper.SerializeWithType(value.PermissionDependency);

                writer.WriteStringValue(content);
                // writer.WriteStringValue(JsonSerializationHelper.SerializeWithType(value.PermissionDependency, typeof(AbpZeroTemplateSimplePermissionDependency)));
            }

            if (value.UsedWidgetFilters != null)
            {
                writer.WritePropertyName(nameof(WidgetDefinition.UsedWidgetFilters));
                writer.WriteStringValue(JsonSerializationHelper.SerializeWithType(value.UsedWidgetFilters));
            }

            writer.WritePropertyName(nameof(WidgetDefinition.Description));
            writer.WriteStringValue(value.Description);

            writer.WritePropertyName(nameof(WidgetDefinition.AllowMultipleInstanceInSamePage));
            writer.WriteBooleanValue(value.AllowMultipleInstanceInSamePage);

            if (value.FeatureDependency != null)
            {
                writer.WritePropertyName(nameof(WidgetDefinition.FeatureDependency));
                writer.WriteStringValue(JsonSerializationHelper.SerializeWithType(value.FeatureDependency));
            }

            writer.WriteEndObject();
        }
    }
}
