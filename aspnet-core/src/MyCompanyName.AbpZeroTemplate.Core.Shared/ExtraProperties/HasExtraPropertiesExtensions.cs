using System;
using System.ComponentModel;
using System.Globalization;
using Abp;
using Abp.Collections.Extensions;

namespace MyCompanyName.AbpZeroTemplate.ExtraProperties
{
    public static class HasExtraPropertiesExtensions
    {
        public static object GetProperty(this IHasExtraProperties source, string name, object defaultValue = null)
        {
            return source.ExtraProperties?.GetOrDefault(name)
                   ?? defaultValue;
        }

        public static TProperty GetProperty<TProperty>(this IHasExtraProperties source, string name,
            TProperty defaultValue = default)
        {
            var value = source.GetProperty(name);

            if (value is null)
            {
                return defaultValue;
            }

            if (!IsPrimitive(typeof(TProperty), includeEnums: true))
                throw new AbpException(
                    "GetProperty<TProperty> does not support non-primitive types. Use non-generic GetProperty method and handle type casting manually.");

            var conversionType = typeof(TProperty);

            if (conversionType == typeof(Guid))
            {
                return (TProperty)TypeDescriptor.GetConverter(conversionType)
                    .ConvertFromInvariantString(value.ToString());
            }

            if (conversionType.IsEnum)
            {
                return (TProperty)value;
            }

            return (TProperty)Convert.ChangeType(value, conversionType, CultureInfo.InvariantCulture);
        }

        public static TSource SetProperty<TSource>(
            this TSource source,
            string name,
            object value)
            where TSource : IHasExtraProperties
        {
            source.ExtraProperties[name] = value;

            return source;
        }

        public static TSource RemoveProperty<TSource>(this TSource source, string name)
            where TSource : IHasExtraProperties
        {
            source.ExtraProperties.Remove(name);
            return source;
        }

        private static bool IsPrimitive(Type type, bool includeEnums)
        {
            if (type.IsPrimitive)
            {
                return true;
            }

            if (includeEnums && type.IsEnum)
            {
                return true;
            }

            return type == typeof(string) ||
                   type == typeof(decimal) ||
                   type == typeof(DateTime) ||
                   type == typeof(DateTimeOffset) ||
                   type == typeof(TimeSpan) ||
                   type == typeof(Guid);
        }

    }
}