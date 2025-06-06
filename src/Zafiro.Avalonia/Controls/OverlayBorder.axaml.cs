using Avalonia.Media;

namespace Zafiro.Avalonia.Controls;

public class OverlayBorder : ContentControl
{
    public static readonly StyledProperty<IBrush> BorderBrushProperty =
        AvaloniaProperty.Register<OverlayBorder, IBrush>(nameof(BorderBrush));

    public static readonly StyledProperty<Thickness> BorderThicknessProperty =
        AvaloniaProperty.Register<OverlayBorder, Thickness>(nameof(BorderThickness));

    public static readonly StyledProperty<CornerRadius> CornerRadiusProperty =
        AvaloniaProperty.Register<OverlayBorder, CornerRadius>(nameof(CornerRadius));

    public static readonly StyledProperty<BoxShadows> BoxShadowProperty =
        AvaloniaProperty.Register<OverlayBorder, BoxShadows>(nameof(BoxShadow));

    public IBrush BorderBrush
    {
        get => GetValue(BorderBrushProperty);
        set => SetValue(BorderBrushProperty, value);
    }

    public Thickness BorderThickness
    {
        get => GetValue(BorderThicknessProperty);
        set => SetValue(BorderThicknessProperty, value);
    }

    public CornerRadius CornerRadius
    {
        get => GetValue(CornerRadiusProperty);
        set => SetValue(CornerRadiusProperty, value);
    }

    public BoxShadows BoxShadow
    {
        get => GetValue(BoxShadowProperty);
        set => SetValue(BoxShadowProperty, value);
    }
}