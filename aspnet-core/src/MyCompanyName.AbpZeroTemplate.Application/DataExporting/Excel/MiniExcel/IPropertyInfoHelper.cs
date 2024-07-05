#nullable enable
using System;
using System.Reflection;
using Abp.Dependency;

namespace MyCompanyName.AbpZeroTemplate.DataExporting.Excel.MiniExcel;

public interface IPropertyInfoHelper : ITransientDependency
{
    object? GetConvertedPropertyValue(PropertyInfo property, object item,
        Func<PropertyInfo, object, object?>? handleComplexTypes = null);
}