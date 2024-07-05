#nullable enable
using System;
using System.Reflection;
using Abp.Runtime.Session;
using Abp.Timing.Timezone;

namespace MyCompanyName.AbpZeroTemplate.DataExporting.Excel.MiniExcel;

public class PropertyInfoHelper : IPropertyInfoHelper
{
    private readonly IAbpSession _abpSession;
    private readonly ITimeZoneConverter _timeZoneConverter;

    public PropertyInfoHelper(IAbpSession abpSession, ITimeZoneConverter timeZoneConverter)
    {
        _abpSession = abpSession;
        _timeZoneConverter = timeZoneConverter;
    }
    
    public object? GetConvertedPropertyValue(PropertyInfo property, object item, Func<PropertyInfo, object, object?>? handleComplexTypes = null)
    {
        var propertyType = property.PropertyType;

        if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            // It's a nullable type, get the underlying type
            propertyType = Nullable.GetUnderlyingType(propertyType);
        }

        var typeCode = Type.GetTypeCode(propertyType);

        switch (typeCode)
        {
            case TypeCode.DateTime:
                // Handle DateTime type (nullable or non-nullable)
                // Example using a custom _timeZoneConverter.Convert method
                if (propertyType == typeof(DateTime))
                {
                    var value = property.GetValue(item);

                    if (value is null)
                    {
                        return "";
                    }

                    return _timeZoneConverter.Convert((DateTime)value, _abpSession.TenantId, _abpSession.GetUserId());
                }

                break;

            case TypeCode.Object:
                if (propertyType != null)
                {
                    if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                    {
                        return property.GetValue(item);
                    }

                    return handleComplexTypes?.Invoke(property, item);
                }
                break;

            // Handle other specific types if needed

            case TypeCode.Empty:
            case TypeCode.DBNull:
            case TypeCode.Boolean:
            case TypeCode.Char:
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
            case TypeCode.String:
            default:
                return property.GetValue(item);
        }

        return null;
    }
}