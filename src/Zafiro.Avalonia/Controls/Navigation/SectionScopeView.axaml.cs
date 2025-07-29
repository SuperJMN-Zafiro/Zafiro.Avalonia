using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Zafiro.UI.Navigation;

namespace Zafiro.Avalonia.Controls.Navigation;

public class SectionScopeView : TemplatedControl
{
    public static readonly StyledProperty<ISectionScope> SectionScopeProperty = AvaloniaProperty.Register<SectionScopeView, ISectionScope>(
        nameof(SectionScope));

    public static readonly StyledProperty<object> BackButtonContentProperty = AvaloniaProperty.Register<SectionScopeView, object>(
        nameof(BackButtonContent));

    public static readonly StyledProperty<bool> IsBackButtonVisibleProperty = AvaloniaProperty.Register<SectionScopeView, bool>(
        nameof(IsBackButtonVisible), true);

    public ISectionScope SectionScope
    {
        get => GetValue(SectionScopeProperty);
        set => SetValue(SectionScopeProperty, value);
    }

    public object BackButtonContent
    {
        get => GetValue(BackButtonContentProperty);
        set => SetValue(BackButtonContentProperty, value);
    }

    public bool IsBackButtonVisible
    {
        get => GetValue(IsBackButtonVisibleProperty);
        set => SetValue(IsBackButtonVisibleProperty, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        SectionScope?.Dispose();
        base.OnUnloaded(e);
    }
}