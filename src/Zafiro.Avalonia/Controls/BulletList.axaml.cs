namespace Zafiro.Avalonia.Controls;

public class BulletList : ItemsControl
{
    public static readonly StyledProperty<object> HeaderProperty = AvaloniaProperty.Register<BulletList, object>(
        nameof(Header));

    public static readonly StyledProperty<object> BulletProperty = AvaloniaProperty.Register<BulletList, object>(
        nameof(Bullet));

    public static readonly StyledProperty<Thickness> HeaderPaddingProperty = AvaloniaProperty.Register<BulletList, Thickness>(
        nameof(HeaderPadding));

    public object Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public object Bullet
    {
        get => GetValue(BulletProperty);
        set => SetValue(BulletProperty, value);
    }

    public Thickness HeaderPadding
    {
        get => GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }
}