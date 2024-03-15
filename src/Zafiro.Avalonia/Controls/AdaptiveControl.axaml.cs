using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Templates;

namespace Zafiro.Avalonia.Controls;

public class AdaptiveControl : TemplatedControl
{
    public static readonly StyledProperty<ControlTemplate> HorizontalTemplateProperty = AvaloniaProperty.Register<AdaptiveControl, ControlTemplate>(
        nameof(HorizontalTemplate));

    public static readonly StyledProperty<ControlTemplate> VerticalTemplateProperty = AvaloniaProperty.Register<AdaptiveControl, ControlTemplate>(
        nameof(VerticalTemplate));

    public AdaptiveControl()
    {
        Observable.FromEventPattern(handler => this.LayoutUpdated += handler, handler => LayoutUpdated -= handler)
            .Select(_ => Bounds.Width > 400)
            .Do(isHorizontal => IsHorizontal = isHorizontal)
            .Subscribe();
    }

    public ControlTemplate HorizontalTemplate
    {
        get => GetValue(HorizontalTemplateProperty);
        set => SetValue(HorizontalTemplateProperty, value);
    }

    public ControlTemplate VerticalTemplate
    {
        get => GetValue(VerticalTemplateProperty);
        set => SetValue(VerticalTemplateProperty, value);
    }

    public static readonly StyledProperty<bool> IsHorizontalProperty = AvaloniaProperty.Register<AdaptiveControl, bool>(nameof(IsHorizontal));

    public bool IsHorizontal
    {
        get => GetValue(IsHorizontalProperty);
        set => SetValue(IsHorizontalProperty, value);
    }
}