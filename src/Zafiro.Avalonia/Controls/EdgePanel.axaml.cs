using Avalonia.Controls.Templates;

namespace Zafiro.Avalonia.Controls;

public class EdgePanel : ContentControl
{
    public static readonly StyledProperty<object> StartContentProperty = AvaloniaProperty.Register<EdgePanel, object>(
        nameof(StartContent));

    public static readonly StyledProperty<object> EndContentProperty = AvaloniaProperty.Register<EdgePanel, object>(
        nameof(EndContent));

    public static readonly StyledProperty<double> SpacingProperty = AvaloniaProperty.Register<EdgePanel, double>(
        nameof(Spacing));

    public static readonly StyledProperty<IDataTemplate> StartContentTemplateProperty = AvaloniaProperty.Register<EdgePanel, IDataTemplate>(
        nameof(StartContentTemplate));

    public static readonly StyledProperty<IDataTemplate> EndContentTemplateProperty = AvaloniaProperty.Register<EdgePanel, IDataTemplate>(
        nameof(EndContentTemplate));

    public object StartContent
    {
        get => GetValue(StartContentProperty);
        set => SetValue(StartContentProperty, value);
    }

    public object EndContent
    {
        get => GetValue(EndContentProperty);
        set => SetValue(EndContentProperty, value);
    }

    public double Spacing
    {
        get => GetValue(SpacingProperty);
        set => SetValue(SpacingProperty, value);
    }

    public IDataTemplate StartContentTemplate
    {
        get => GetValue(StartContentTemplateProperty);
        set => SetValue(StartContentTemplateProperty, value);
    }

    public IDataTemplate EndContentTemplate
    {
        get => GetValue(EndContentTemplateProperty);
        set => SetValue(EndContentTemplateProperty, value);
    }
}