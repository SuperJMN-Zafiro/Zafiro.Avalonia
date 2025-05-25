namespace Zafiro.Avalonia.Controls;

public class Card : ContentControl
{
    public static readonly StyledProperty<object> HeaderStartContentProperty = AvaloniaProperty.Register<Card, object>(
        nameof(HeaderStartContent));

    public static readonly StyledProperty<object> HeaderEndContentProperty = AvaloniaProperty.Register<Card, object>(
        nameof(HeaderEndContent));

    public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<Card, object>(
        nameof(Header));

    public static readonly StyledProperty<object> SubheaderProperty = AvaloniaProperty.Register<Card, object>(
        nameof(Subheader));

    public static readonly StyledProperty<Thickness> ContentPaddingProperty = AvaloniaProperty.Register<Card, Thickness>(
        nameof(ContentPadding));

    public static readonly StyledProperty<Thickness> HeaderPaddingProperty = AvaloniaProperty.Register<Card, Thickness>(
        nameof(HeaderPadding));

    public static readonly StyledProperty<double> HeaderSpacingProperty = AvaloniaProperty.Register<Card, double>(
        nameof(HeaderSpacing));

    public static readonly StyledProperty<double> HeaderSubheaderSpacingProperty = AvaloniaProperty.Register<Card, double>(
        nameof(HeaderSubheaderSpacing));

    public object HeaderStartContent
    {
        get => GetValue(HeaderStartContentProperty);
        set => SetValue(HeaderStartContentProperty, value);
    }

    public object HeaderEndContent
    {
        get => GetValue(HeaderEndContentProperty);
        set => SetValue(HeaderEndContentProperty, value);
    }

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object Subheader
    {
        get => GetValue(SubheaderProperty);
        set => SetValue(SubheaderProperty, value);
    }

    public Thickness ContentPadding
    {
        get => GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    public Thickness HeaderPadding
    {
        get => GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }

    public double HeaderSpacing
    {
        get => GetValue(HeaderSpacingProperty);
        set => SetValue(HeaderSpacingProperty, value);
    }

    public double HeaderSubheaderSpacing
    {
        get => GetValue(HeaderSubheaderSpacingProperty);
        set => SetValue(HeaderSubheaderSpacingProperty, value);
    }
}