using System;
using Abp;

namespace MyCompanyName.AbpZeroTemplate.ExtraProperties
{

    public static class ExtraPropertyDictionaryExtensions
    {
        public static T ToEnum<T>(this ExtraPropertyDictionary extraPropertyDictionary, string key)
            where T : Enum
        {
            if (extraPropertyDictionary[key].GetType() == typeof(T))
            {
                return (T)extraPropertyDictionary[key];
            }

            extraPropertyDictionary[key] = Enum.Parse(typeof(T),
                extraPropertyDictionary[key].ToString() ??
                throw new AbpException($"Extra property value for '{key}' is null!"), ignoreCase: true);

            return (T)extraPropertyDictionary[key];
        }
    }
}