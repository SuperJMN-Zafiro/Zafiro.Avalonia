using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls;

public class FlexEdgePanel : TemplatedControl
{
    public static readonly StyledProperty<object> MainContentProperty = AvaloniaProperty.Register<FlexEdgePanel, object>(
        nameof(MainContent));

    public object MainContent
    {
        get => GetValue(MainContentProperty);
        set => SetValue(MainContentProperty, value);
    }

    public static readonly StyledProperty<object> StartContentProperty = AvaloniaProperty.Register<FlexEdgePanel, object>(
        nameof(StartContent));

    public object StartContent
    {
        get => GetValue(StartContentProperty);
        set => SetValue(StartContentProperty, value);
    }

    public static readonly StyledProperty<object> EndContentProperty = AvaloniaProperty.Register<FlexEdgePanel, object>(
        nameof(EndContent));

    public object EndContent
    {
        get => GetValue(EndContentProperty);
        set => SetValue(EndContentProperty, value);
    }
}