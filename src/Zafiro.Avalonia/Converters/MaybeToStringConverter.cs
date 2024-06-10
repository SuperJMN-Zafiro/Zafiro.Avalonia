using System.Globalization;
using Avalonia.Data.Converters;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Converters;

public class MaybeToStringConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Maybe<string> maybeValue)
        {
            return maybeValue.HasValue ? maybeValue.Value : string.Empty;
        }
        return string.Empty;
    }
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var stringValue = value as string;
        return string.IsNullOrEmpty(stringValue) ? Maybe<string>.None : Maybe<string>.From(stringValue);
    }
}