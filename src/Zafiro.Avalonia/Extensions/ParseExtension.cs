using System.ComponentModel;
using Avalonia.Markup.Xaml;

namespace Zafiro.Avalonia.Extensions;

public class ParseExtension : MarkupExtension
{
    public ParseExtension()
    {
    }

    public ParseExtension(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
    public Type Type { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Value) || Type == null)
            return null;

        try
        {
            // Handle enums specially for flags support
            if (Type.IsEnum)
            {
                return ParseEnum(Value, Type);
            }

            // Use TypeConverter for other types
            var converter = TypeDescriptor.GetConverter(Type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                return converter.ConvertFromString(Value);
            }

            // Fallback to Convert.ChangeType
            return Convert.ChangeType(Value, Type);
        }
        catch
        {
            return Activator.CreateInstance(Type); // Return default value on error
        }
    }

    private object ParseEnum(string value, Type enumType)
    {
        // Handle combined flags like "Left|Top" or "Left,Top"
        var parts = value.Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 1)
        {
            return Enum.Parse(enumType, value.Trim());
        }

        // Combine multiple flags
        var result = Activator.CreateInstance(enumType);
        foreach (var part in parts)
        {
            var enumValue = Enum.Parse(enumType, part.Trim());
            result = CombineFlags(result, enumValue);
        }

        return result;
    }

    private object CombineFlags(object current, object value)
    {
        var currentInt = Convert.ToInt64(current);
        var valueInt = Convert.ToInt64(value);
        var combined = currentInt | valueInt;
        return Enum.ToObject(current.GetType(), combined);
    }
}