using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Zafiro.UI.Navigation;

namespace Zafiro.Avalonia.Controls.Navigation;

public partial class NavigationHostView : UserControl
{
    public NavigationHostView()
    {
        InitializeComponent();
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        NavigationHost.Dispose();
        base.OnUnloaded(e);
    }

    public static readonly StyledProperty<INavigationHost> NavigationHostProperty = AvaloniaProperty.Register<NavigationHostView, INavigationHost>(
        nameof(NavigationHost));

    public INavigationHost NavigationHost
    {
        get => GetValue(NavigationHostProperty);
        set => SetValue(NavigationHostProperty, value);
    }
}