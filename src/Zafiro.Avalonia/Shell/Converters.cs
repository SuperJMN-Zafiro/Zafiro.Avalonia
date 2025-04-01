using Avalonia.Data.Converters;
using Zafiro.UI.Navigation.Sections;
using Section = Zafiro.UI.Navigation.Sections.Section;

namespace Zafiro.Avalonia.Shell;

public class Converters
{
    // public static readonly FuncValueConverter<string, object> StringToIcon = new(str =>
    // {
    //     if (str is null)
    //     {
    //         return AvaloniaProperty.UnsetValue;
    //     }
    //
    //     var prefix = str.Split(":");
    //     if (prefix[0] == "svg")
    //     {
    //         return new Avalonia.Svg.Svg(new Uri("avares://DNAGedcom.Insight.App"))
    //         {
    //             Path = prefix[1]
    //         };
    //     }
    //
    //     return new Icon
    //     {
    //         Value = str
    //     };
    // });

    public static readonly FuncValueConverter<Section, bool> IsActivatable = new(sectionBase => sectionBase is not SectionSeparator);

    public static readonly FuncValueConverter<bool, Dock> IsPrimaryToDock = new(isPrimary => isPrimary ? Dock.Top : Dock.Bottom);
}