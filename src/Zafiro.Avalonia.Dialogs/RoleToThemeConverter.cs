using System.Globalization;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Dialogs;

public class RoleToClassesConverter : IMultiValueConverter
{
    public static RoleToClassesConverter Instance { get; } = new();

    public object Convert(IList<object> values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values[0] is OptionRole role)
        {
            return role switch
            {
                OptionRole.Primary => new[] { "primary" },
                OptionRole.Destructive => new[] { "destructive" },
                OptionRole.Secondary => new[] { "secondary" },
                _ => Array.Empty<string>()
            };
        }
        
        return Array.Empty<string>();
    }
}