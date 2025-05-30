using Avalonia.Controls.Templates;
using Avalonia.Media;
using Avalonia.Styling;

namespace Zafiro.Avalonia.Controls;

public class ShellSplitView : ContentControl
{
    public static readonly StyledProperty<bool> IsPaneOpenProperty = AvaloniaProperty.Register<ShellSplitView, bool>(
        nameof(IsPaneOpen));

    public static readonly StyledProperty<ShellSplitViewDisplayMode> DisplayModeProperty = AvaloniaProperty.Register<ShellSplitView, ShellSplitViewDisplayMode>(
        nameof(DisplayMode));

    public static readonly StyledProperty<object> PaneContentProperty = AvaloniaProperty.Register<ShellSplitView, object>(
        nameof(PaneContent));

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

    public static readonly StyledProperty<ControlTheme> ToggleButtonThemeProperty = AvaloniaProperty.Register<ShellSplitView, ControlTheme>(
        nameof(ToggleButtonTheme));

    public static readonly StyledProperty<object> ContentHeaderProperty = AvaloniaProperty.Register<ShellSplitView, object>(
        nameof(ContentHeader));

    public static readonly StyledProperty<IDataTemplate> ContentHeaderTemplateProperty = AvaloniaProperty.Register<ShellSplitView, IDataTemplate>(
        nameof(ContentHeaderTemplate));

    public static readonly StyledProperty<IDataTemplate> PaneContentTemplateProperty = AvaloniaProperty.Register<ShellSplitView, IDataTemplate>(
        nameof(PaneContentTemplate));

    public static readonly StyledProperty<IBrush> PaneBorderBrushProperty = AvaloniaProperty.Register<ShellSplitView, IBrush>(
        nameof(PaneBorderBrush));

    public static readonly StyledProperty<Thickness> PaneBorderThicknessProperty = AvaloniaProperty.Register<ShellSplitView, Thickness>(
        nameof(PaneBorderThickness));

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

    public object PaneContent
    {
        get => GetValue(PaneContentProperty);
        set => SetValue(PaneContentProperty, value);
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

    public ControlTheme ToggleButtonTheme
    {
        get => GetValue(ToggleButtonThemeProperty);
        set => SetValue(ToggleButtonThemeProperty, value);
    }

    public object ContentHeader
    {
        get => GetValue(ContentHeaderProperty);
        set => SetValue(ContentHeaderProperty, value);
    }

    public IDataTemplate ContentHeaderTemplate
    {
        get => GetValue(ContentHeaderTemplateProperty);
        set => SetValue(ContentHeaderTemplateProperty, value);
    }

    public IDataTemplate PaneContentTemplate
    {
        get => GetValue(PaneContentTemplateProperty);
        set => SetValue(PaneContentTemplateProperty, value);
    }

    public IBrush PaneBorderBrush
    {
        get => GetValue(PaneBorderBrushProperty);
        set => SetValue(PaneBorderBrushProperty, value);
    }

    public Thickness PaneBorderThickness
    {
        get => GetValue(PaneBorderThicknessProperty);
        set => SetValue(PaneBorderThicknessProperty, value);
    }
}

public enum ShellSplitViewDisplayMode
{
    Inline,
    Overlay,
    CompactOverlay,
    CompactInline
}