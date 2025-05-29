using Avalonia.Data.Converters;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public static class ShellConverters
{
    public static readonly FuncValueConverter<Section, bool> IsActivatable = new(sectionBase => sectionBase is not SectionSeparator);

    public static readonly FuncValueConverter<bool, Dock> IsPrimaryToDock = new(isPrimary => isPrimary ? Dock.Top : Dock.Bottom);
    public static readonly FuncValueConverter<ShellSplitViewDisplayMode, bool> IsOverlay = new(displayMode => displayMode == ShellSplitViewDisplayMode.Overlay || displayMode == ShellSplitViewDisplayMode.CompactOverlay);
}