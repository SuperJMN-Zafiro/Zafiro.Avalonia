using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public class ResourceKeyConverter : AvaloniaObject, IValueConverter
{
    public static readonly StyledProperty<ResourceDictionary> DictionaryProperty = AvaloniaProperty.Register<ResourceKeyConverter, ResourceDictionary>(nameof(Dictionary));

    public ResourceDictionary Dictionary
    {
        get => GetValue(DictionaryProperty);
        set => SetValue(DictionaryProperty, value);
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is not null ? Dictionary[value] : BindingValueType.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException();
}