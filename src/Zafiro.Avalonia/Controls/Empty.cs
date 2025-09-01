using Avalonia.Markup.Xaml.Templates;
using Avalonia.Styling;

namespace Zafiro.Avalonia.Controls;

public class Empty
{
    public static readonly AttachedProperty<object> ContentProperty = AvaloniaProperty.RegisterAttached<Empty, Control, object>("Content", inherits: true);

    public static readonly AttachedProperty<ControlTheme> ContentThemeProperty = AvaloniaProperty.RegisterAttached<Empty, Visual, ControlTheme>("ContentTheme", inherits: true);

    public static readonly AttachedProperty<DataTemplate> ContentTemplateProperty = AvaloniaProperty.RegisterAttached<Empty, Visual, DataTemplate>("ContentTemplate", inherits: true);

    public static readonly AttachedProperty<Thickness> MarginProperty =
        AvaloniaProperty.RegisterAttached<Empty, Visual, Thickness>("Margin");

    public static void SetContent(Visual obj, object value) => obj.SetValue(ContentProperty, value);
    public static object GetContent(Visual obj) => obj.GetValue(ContentProperty);

    public static void SetContentTheme(Visual obj, ControlTheme value) => obj.SetValue(ContentThemeProperty, value);
    public static ControlTheme GetContentTheme(Visual obj) => obj.GetValue(ContentThemeProperty);

    public static void SetContentTemplate(Visual obj, DataTemplate value) => obj.SetValue(ContentTemplateProperty, value);
    public static DataTemplate GetContentTemplate(Visual obj) => obj.GetValue(ContentTemplateProperty);

    public static void SetMargin(Visual obj, Thickness value) => obj.SetValue(MarginProperty, value);
    public static Thickness GetMargin(Visual obj) => obj.GetValue(MarginProperty);
}