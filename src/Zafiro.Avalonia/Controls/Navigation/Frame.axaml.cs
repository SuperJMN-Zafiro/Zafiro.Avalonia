using Avalonia.Controls.Primitives;
using Zafiro.UI.Navigation;

namespace Zafiro.Avalonia.Controls.Navigation;

public class Frame : TemplatedControl
{
    public static readonly DirectProperty<Frame, INavigator?> NavigatorProperty = AvaloniaProperty.RegisterDirect<Frame, INavigator?>(
        nameof(Navigator), o => o.Navigator, (o, v) => o.Navigator = v);

    public static readonly StyledProperty<bool> IsBackButtonVisibleProperty =
        AvaloniaProperty.Register<Frame, bool>(
            nameof(IsBackButtonVisible),
            defaultValue: true);

    public static readonly StyledProperty<object> BackButtonContentProperty =
        AvaloniaProperty.Register<Frame, object>(
            nameof(BackButtonContent));

    private INavigator? navigator;

    public INavigator? Navigator
    {
        get => navigator;
        set => SetAndRaise(NavigatorProperty, ref navigator, value);
    }

    public bool IsBackButtonVisible
    {
        get => GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    public object BackButtonContent
    {
        get => GetValue(BackButtonContentProperty);
        set => SetValue(BackButtonContentProperty, value);
    }
}