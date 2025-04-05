using Avalonia.Data.Converters;
using Zafiro.UI.Navigation.Sections;
using Section = Zafiro.UI.Navigation.Sections.Section;

namespace Zafiro.Avalonia.Shell;

public class Converters
{
    public static readonly FuncValueConverter<Section, bool> IsActivatable = new(sectionBase => sectionBase is not SectionSeparator);
    public static readonly FuncValueConverter<bool, Dock> IsPrimaryToDock = new(isPrimary => isPrimary ? Dock.Top : Dock.Bottom);
}