namespace Zafiro.Avalonia.Controls;

public class Pane : ContentControl
{
    public static readonly StyledProperty<object> HeaderStartContentProperty = AvaloniaProperty.Register<Pane, object>(
        nameof(HeaderStartContent));

    public object HeaderStartContent
    {
        get => GetValue(HeaderStartContentProperty);
        set => SetValue(HeaderStartContentProperty, value);
    }

    public static readonly StyledProperty<object> HeaderEndContentProperty = AvaloniaProperty.Register<Pane, object>(
        nameof(HeaderEndContent));

    public object HeaderEndContent
    {
        get => GetValue(HeaderEndContentProperty);
        set => SetValue(HeaderEndContentProperty, value);
    }

    public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<Pane, object>(
        nameof(Header));

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly StyledProperty<object> SubheaderProperty = AvaloniaProperty.Register<Pane, object>(
        nameof(Subheader));

    public object Subheader
    {
        get => GetValue(SubheaderProperty);
        set => SetValue(SubheaderProperty, value);
    }

    public static readonly StyledProperty<Thickness> ContentPaddingProperty = AvaloniaProperty.Register<Pane, Thickness>(
        nameof(ContentPadding));

    public Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    public static readonly StyledProperty<Thickness> HeaderPaddingProperty = AvaloniaProperty.Register<Pane, Thickness>(
        nameof(HeaderPadding));

    public Thickness HeaderPadding
    {
        get => GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }
}