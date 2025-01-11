using System;
using System.ComponentModel;
using System.Reflection;

namespace Demo.BLL.Helpers.Converter;

public static class EnumConverter
{
    public static T StringToEnum<T>(this string stringValue) where T : struct
    {
        T enumValue;
        if (Enum.TryParse(stringValue, true, out enumValue))
            return enumValue;
        return (T)GetDefaultValue(typeof(T));
    }

    private static object GetDefaultValue(Type enumType)
    {
        var attribute = enumType.GetCustomAttribute<DefaultValueAttribute>(false);
        if (attribute != null)
            return attribute.Value;

        var innerType = enumType.GetEnumUnderlyingType();
        var zero = Activator.CreateInstance(innerType);
        if (enumType.IsEnumDefined(zero))
            return zero;

        var values = enumType.GetEnumValues();
        return values.GetValue(0);
    }
}