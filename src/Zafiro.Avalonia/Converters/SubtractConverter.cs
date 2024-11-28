using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public class SubtractConverter
{
    public static FuncMultiValueConverter<double, double> Instance = new(d =>
    {
        var list = d.ToList();
        return list[1] - list[0];
    });
}