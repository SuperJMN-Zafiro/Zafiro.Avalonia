using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Templates;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls;

public class AdaptiveControl : TemplatedControl
{
    public static readonly StyledProperty<ControlTemplate> HorizontalTemplateProperty = AvaloniaProperty.Register<AdaptiveControl, ControlTemplate>(
        nameof(HorizontalTemplate));

    public static readonly StyledProperty<ControlTemplate> VerticalTemplateProperty = AvaloniaProperty.Register<AdaptiveControl, ControlTemplate>(
        nameof(VerticalTemplate));

    public AdaptiveControl()
    {
        Observable.FromEventPattern(handler => this.LayoutUpdated += handler, handler => LayoutUpdated -= handler)
            .Select(_ => Bounds.Width > 400)
            .Do(isHorizontal =>
            {
                Template = isHorizontal ? HorizontalTemplate: VerticalTemplate;
                Orientation = isHorizontal ? Orientation.Horizontal : Orientation.Vertical;
            })
            .Subscribe();

        Observable.Interval(TimeSpan.FromSeconds(1), RxApp.MainThreadScheduler).Do(l => InvalidateVisual()).Subscribe();

        AffectsRender<AdaptiveControl>(OrientationProperty);
    }

    public ControlTemplate HorizontalTemplate
    {
        get => GetValue(HorizontalTemplateProperty);
        set => SetValue(HorizontalTemplateProperty, value);
    }

    public ControlTemplate VerticalTemplate
    {
        get => GetValue(VerticalTemplateProperty);
        set => SetValue(VerticalTemplateProperty, value);
    }

    public static readonly StyledProperty<Orientation> OrientationProperty = AvaloniaProperty.Register<AdaptiveControl, Orientation>(
        nameof(Orientation));

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }
}