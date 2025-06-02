using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Metadata;
using Avalonia.Styling;

namespace Zafiro.Avalonia.Converters;

public class ControlThemeConverter : IValueConverter
{
    [Content]
    // ReSharper disable once CollectionNeverUpdated.Global
    public Dictionary<object, ControlTheme> Themes { get; } = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null)
        {
            return BindingOperations.DoNothing;
        }

        return Themes.TryGetValue(value, out var controlTheme) ? controlTheme : BindingOperations.DoNothing;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}