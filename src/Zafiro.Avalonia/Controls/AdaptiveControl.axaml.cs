using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.Templates;

namespace Zafiro.Avalonia.Controls;

public class AdaptiveControl : TemplatedControl
{
    public static readonly StyledProperty<ControlTemplate> HorizontalTemplateProperty = AvaloniaProperty.Register<AdaptiveControl, ControlTemplate>(
        nameof(HorizontalTemplate));

    public static readonly StyledProperty<ControlTemplate> VerticalTemplateProperty = AvaloniaProperty.Register<AdaptiveControl, ControlTemplate>(
        nameof(VerticalTemplate));

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
}