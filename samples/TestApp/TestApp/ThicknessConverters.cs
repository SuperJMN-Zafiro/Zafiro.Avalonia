using System;
using System.Globalization;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace TestApp;

public static class ThicknessConverters
{
    public static FuncMultiValueConverter<double, Thickness> DoubleToHorizontalThicknessConverter { get; } = new(d => new Thickness(d.First(), 0));
    public static FuncMultiValueConverter<double, Thickness> DoubleToVerticalThicknessConverter { get; } = new(d => new Thickness(0, d.First()));
    public static FuncMultiValueConverter<object, Thickness> RowsMarginConverter { get; } = new(d =>
    {
        var list = d.ToList();
        
        if (list.Any(o => o is UnsetValueType))
        {
            return new Thickness();
        }
        
        var height = (double)list[0];
        var rows = (int)list[1];
        
        return new Thickness(0, height / rows / 2);
    });
}

public class ConstantConverter : IValueConverter
{
    public static ConstantConverter Instance { get; } = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return parameter;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}