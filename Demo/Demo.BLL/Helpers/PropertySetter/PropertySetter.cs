using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.Domain.Entities.Demo;
using Demo.Domain.Models.HelperModels.MeasurementModels;

namespace Demo.BLL.Helpers.PropertySetter;

internal static class PropertySetter
{
    public static DemoSaveModel SetProperty(this DemoSaveModel demo,
        ICollection<PropertyMapper> propertyMappers, string columnName, string data, IClock clock)
    {
        var mapper =
            propertyMappers.FirstOrDefault(x =>
                x.PropertyName.ToLower().Trim() == columnName.ToLower().Trim());

        if (mapper != null)
        {
            data = data.Trim();
            var prop = demo.GetType()
                .GetProperty(mapper.ColumnName,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                var val = ParseValueToData(prop, clock, data);

                prop.SetValue(demo, val, null);
            }
        }

        return demo;
    }

    private static object ParseValueToData(PropertyInfo prop, IClock clock, string data)
    {
        object val;
        try
        {
            if (prop.PropertyType == typeof(DateTimeOffset))
            {
                var polishCulture = new CultureInfo("pl-PL");

                if (DateTimeOffset.TryParse(data, polishCulture, DateTimeStyles.None,
                        out var results))
                    val = results;
                else
                    val = clock.Current();
            }
            else if (prop.PropertyType == typeof(float))
            {
                data = data.Replace(",", ".");
                float.TryParse(data, NumberStyles.Float, CultureInfo.InvariantCulture, out var newValue);
                val = Convert.ChangeType(newValue, prop.PropertyType);
            }
            else
            {
                val = Convert.ChangeType(data, prop.PropertyType);
            }
        }
        catch
        {
            val = default;
        }

        return val;
    }
}