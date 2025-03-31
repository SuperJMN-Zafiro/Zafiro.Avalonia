using System.Reactive;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Zafiro.UI.Commands;
using Zafiro.UI.Navigation;

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
        private set
        {
            if (content is IDisposable disposable)
            {
                disposable.Dispose();
            }
            
            SetAndRaise(ContentProperty, ref content, value);
        }
    }

    public Frame()
    {
        this.WhenAnyValue(x => x.Navigator.Content).Subscribe(o => Content = o);
        this.WhenAnyValue(x => x.Navigator.Back).Subscribe(o => Back = o);
    }

    public IEnhancedCommand Back { get; set; }

    public static readonly StyledProperty<bool> IsBackButtonVisibleProperty = AvaloniaProperty.Register<Frame, bool>(
        nameof(IsBackButtonVisible), defaultValue: true);

    public bool IsBackButtonVisible
    {
        get => GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    public static readonly StyledProperty<object> BackButtonContentProperty = AvaloniaProperty.Register<Frame, object>(
        nameof(BackButtonContent));

    public object BackButtonContent
    {
        get => GetValue(BackButtonContentProperty);
        set => SetValue(BackButtonContentProperty, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        if (content is IDisposable disposable)
        {
            disposable.Dispose();
        }
        
        base.OnUnloaded(e);
    }
}