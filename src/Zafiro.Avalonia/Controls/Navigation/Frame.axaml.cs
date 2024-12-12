using System.Reactive;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls.Navigation;

public class Frame : TemplatedControl
{
    public static readonly StyledProperty<INavigator> NavigatorProperty = AvaloniaProperty.Register<Frame, INavigator>(
        nameof(Navigator));

    public INavigator Navigator
    {
        get => GetValue(NavigatorProperty);
        set => SetValue(NavigatorProperty, value);
    }

    private object content;

    public static readonly DirectProperty<Frame, object> ContentProperty = AvaloniaProperty.RegisterDirect<Frame, object>(
        nameof(Content), o => o.Content, (o, v) => o.Content = v);

    public object Content
    {
        get => content;
        private set => SetAndRaise(ContentProperty, ref content, value);
    }

    public Frame()
    {
        this.WhenAnyValue(x => x.Navigator.Content).Subscribe(o => Content = o);
        this.WhenAnyValue(x => x.Navigator.Back).Subscribe(o => Back = o);
    }

    public ReactiveCommand<Unit,Unit> Back { get; set; }
}