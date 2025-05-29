using Avalonia.Controls.Templates;
using Avalonia.Media;

namespace Zafiro.Avalonia.Controls;

public class ShellSplitView : ContentControl
{
    public static readonly StyledProperty<bool> IsPaneOpenProperty = AvaloniaProperty.Register<ShellSplitView, bool>(
        nameof(IsPaneOpen));

    public static readonly StyledProperty<ShellSplitViewDisplayMode> DisplayModeProperty = AvaloniaProperty.Register<ShellSplitView, ShellSplitViewDisplayMode>(
        nameof(DisplayMode));

    public static readonly StyledProperty<object> PaneProperty = AvaloniaProperty.Register<ShellSplitView, object>(
        nameof(Pane));

    public static readonly StyledProperty<double> ExpandButtonSizeProperty = AvaloniaProperty.Register<ShellSplitView, double>(
        nameof(ExpandButtonSize));

    public static readonly StyledProperty<double> OpenPaneLengthProperty = AvaloniaProperty.Register<ShellSplitView, double>(
        nameof(OpenPaneLength));

    public static readonly StyledProperty<object> ToggleButtonContentProperty = AvaloniaProperty.Register<ShellSplitView, object>(
        nameof(ToggleButtonContent));

    public static readonly StyledProperty<object> PaneHeaderProperty = AvaloniaProperty.Register<ShellSplitView, object>(
        nameof(PaneHeader));

    public static readonly StyledProperty<IDataTemplate> PaneHeaderTemplateProperty = AvaloniaProperty.Register<ShellSplitView, IDataTemplate>(
        nameof(PaneHeaderTemplate));

    public static readonly StyledProperty<IBrush> PaneBackgroundProperty = AvaloniaProperty.Register<ShellSplitView, IBrush>(
        nameof(PaneBackground));

    public bool IsPaneOpen
    {
        get => GetValue(IsPaneOpenProperty);
        set => SetValue(IsPaneOpenProperty, value);
    }

    public ShellSplitViewDisplayMode DisplayMode
    {
        get => GetValue(DisplayModeProperty);
        set => SetValue(DisplayModeProperty, value);
    }

    public object Pane
    {
        get => GetValue(PaneProperty);
        set => SetValue(PaneProperty, value);
    }

    public double ExpandButtonSize
    {
        get => GetValue(ExpandButtonSizeProperty);
        set => SetValue(ExpandButtonSizeProperty, value);
    }

    public double OpenPaneLength
    {
        get => GetValue(OpenPaneLengthProperty);
        set => SetValue(OpenPaneLengthProperty, value);
    }

    public object ToggleButtonContent
    {
        get => GetValue(ToggleButtonContentProperty);
        set => SetValue(ToggleButtonContentProperty, value);
    }

    public object PaneHeader
    {
        get => GetValue(PaneHeaderProperty);
        set => SetValue(PaneHeaderProperty, value);
    }

    public IDataTemplate PaneHeaderTemplate
    {
        get => GetValue(PaneHeaderTemplateProperty);
        set => SetValue(PaneHeaderTemplateProperty, value);
    }

    public IBrush PaneBackground
    {
        get => GetValue(PaneBackgroundProperty);
        set => SetValue(PaneBackgroundProperty, value);
    }
}

public enum ShellSplitViewDisplayMode
{
    Inline,
    Overlay,
    CompactOverlay,
    CompactInline,
}