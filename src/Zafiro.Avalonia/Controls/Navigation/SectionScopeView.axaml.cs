using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Zafiro.UI.Navigation;

namespace Zafiro.Avalonia.Controls.Navigation;

public partial class SectionScopeView : TemplatedControl
{
    protected override void OnUnloaded(RoutedEventArgs e)
    {
        SectionScope.Dispose();
        base.OnUnloaded(e);
    }

    public static readonly StyledProperty<ISectionScope> SectionScopeProperty = AvaloniaProperty.Register<SectionScopeView, ISectionScope>(
        nameof(SectionScope));

    public ISectionScope SectionScope
    {
        get => GetValue(SectionScopeProperty);
        set => SetValue(SectionScopeProperty, value);
    }

    public static readonly StyledProperty<object> BackButtonContentProperty = AvaloniaProperty.Register<SectionScopeView, object>(
        nameof(BackButtonContent));

    public object BackButtonContent
    {
        get => GetValue(BackButtonContentProperty);
        set => SetValue(BackButtonContentProperty, value);
    }
}