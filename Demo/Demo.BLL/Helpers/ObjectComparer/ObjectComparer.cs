using System;

namespace Demo.BLL.Helpers.ObjectComparer;

internal static class ObjectComparer
{
    public static bool AreEqual<T>(T obj1, T obj2)
    {
        if (obj1 == null || obj2 == null)
            return false;

        var properties = typeof(T).GetProperties();

        foreach (var property in properties)
            if (IsSimpleType(property.PropertyType) && property.Name.Trim().ToLower() != "id")
            {
                var value1 = property.GetValue(obj1);
                var value2 = property.GetValue(obj2);

                if (!value1.Equals(value2))
                    return false;
            }

        return true;
    }

    private static bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || type.IsEnum || type == typeof(string) ||
               type == typeof(decimal) || type == typeof(DateTime) ||
               type == typeof(float) || type == typeof(uint) ||
               type == typeof(ushort) || type == typeof(DateTimeOffset) || type == typeof(TimeSpan) ||
               (Nullable.GetUnderlyingType(type) != null && Nullable.GetUnderlyingType(type).IsEnum);
    }
}