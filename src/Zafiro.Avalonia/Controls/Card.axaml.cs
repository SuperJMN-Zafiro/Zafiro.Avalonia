using Avalonia.Controls.Templates;
using Avalonia.Layout;

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

    public static readonly StyledProperty<VerticalAlignment> HeaderVerticalAlignmentProperty = AvaloniaProperty.Register<Card, VerticalAlignment>(
        nameof(HeaderVerticalAlignment));

    public static readonly StyledProperty<IDataTemplate> HeaderStartContentTemplateProperty = AvaloniaProperty.Register<Card, IDataTemplate>(
        nameof(HeaderStartContentTemplate));

    public static readonly StyledProperty<IDataTemplate> HeaderEndContentTemplateProperty = AvaloniaProperty.Register<Card, IDataTemplate>(
        nameof(HeaderEndContentTemplate));

    public static readonly StyledProperty<IDataTemplate> HeaderTemplateProperty = AvaloniaProperty.Register<Card, IDataTemplate>(
        nameof(HeaderTemplate));

    public static readonly StyledProperty<IDataTemplate> SubheaderTemplateProperty = AvaloniaProperty.Register<Card, IDataTemplate>(
        nameof(SubheaderTemplate));

    public static readonly StyledProperty<HorizontalAlignment> HeaderHorizontalAlignmentProperty = AvaloniaProperty.Register<Card, HorizontalAlignment>(
        nameof(HeaderHorizontalAlignment));

    public VerticalAlignment HeaderVerticalAlignment
    {
        get => GetValue(HeaderVerticalAlignmentProperty);
        set => SetValue(HeaderVerticalAlignmentProperty, value);
    }

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

    public IDataTemplate HeaderStartContentTemplate
    {
        get => GetValue(HeaderStartContentTemplateProperty);
        set => SetValue(HeaderStartContentTemplateProperty, value);
    }

    public IDataTemplate HeaderEndContentTemplate
    {
        get => GetValue(HeaderEndContentTemplateProperty);
        set => SetValue(HeaderEndContentTemplateProperty, value);
    }

    public IDataTemplate HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    public IDataTemplate SubheaderTemplate
    {
        get => GetValue(SubheaderTemplateProperty);
        set => SetValue(SubheaderTemplateProperty, value);
    }

    public HorizontalAlignment HeaderHorizontalAlignment
    {
        get => GetValue(HeaderHorizontalAlignmentProperty);
        set => SetValue(HeaderHorizontalAlignmentProperty, value);
    }
}