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
        SectionScope.Dispose();
        base.OnUnloaded(e);
    }

    public static readonly StyledProperty<ISectionScope> SectionScopeProperty = AvaloniaProperty.Register<NavigationHostView, ISectionScope>(
        nameof(SectionScope));

    public ISectionScope SectionScope
    {
        get => GetValue(SectionScopeProperty);
        set => SetValue(SectionScopeProperty, value);
    }
}