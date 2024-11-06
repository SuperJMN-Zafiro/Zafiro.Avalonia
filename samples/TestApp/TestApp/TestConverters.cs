using Avalonia.Data.Converters;
using Zafiro.Avalonia.DesignTime;

namespace TestApp;

public static class TestConverters
{
    public static readonly FuncValueConverter<object, object> Instance = new FuncValueConverter<object, object>(o => 67d);
}