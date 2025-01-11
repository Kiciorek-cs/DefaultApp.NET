namespace Demo.BLL.Helpers.String;

internal static class StringTrimmer
{
    public static void TrimStrings<T>(this T model)
    {
        var properties = model.GetType().GetProperties();

        foreach (var property in properties)
            if (property.PropertyType == typeof(string))
            {
                var value = (string)property.GetValue(model);

                if (value != null)
                {
                    var trimmedValue = value.Trim();

                    property.SetValue(model, trimmedValue);
                }
            }
    }
}