using System.Reactive.Disposables;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Custom;

namespace Zafiro.Avalonia.Behaviors;

public class ProximityRevealBehavior : AttachedToVisualTreeBehavior<Control>
{
    public static readonly StyledProperty<Thickness> InflateHitboxesByProperty = AvaloniaProperty.Register<ProximityRevealBehavior, Thickness>(nameof(InflateHitBoxesBy));

    public static readonly StyledProperty<bool> ForceVisibleProperty = AvaloniaProperty.Register<ProximityRevealBehavior, bool>(nameof(ForceVisible));

    [ResolveByName] public Visual? Target { get; set; }

    public Thickness InflateHitBoxesBy
    {
        get => GetValue(InflateHitboxesByProperty);
        set => SetValue(InflateHitboxesByProperty, value);
    }

    public bool ForceVisible
    {
        get => GetValue(ForceVisibleProperty);
        set => SetValue(ForceVisibleProperty, value);
    }

    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        var disposable = new CompositeDisposable();

        if (AssociatedObject is null)
        {
            return disposable;
        }

        var mainView = GetMainView();

        if (mainView is null)
        {
            return disposable;
        }

        var pointerPos = Observable
            .FromEventPattern<PointerEventArgs>(handler => mainView.PointerMoved += handler, handler => mainView.PointerMoved -= handler)
            .Select(x => x.EventArgs.GetPosition(mainView));

        var hits = pointerPos.Select(point =>
        {
            if (Target is null)
            {
                return false;
            }

            return ContainsPoint(mainView, point, AssociatedObject) || ContainsPoint(mainView, point, Target);
        });

        var isVisibilityForced = this.WhenAnyValue(x => x.ForceVisible);

        hits.CombineLatest(isVisibilityForced, (isHit, isForced) => (isHit, isForced))
            .Select(tuple => tuple.isHit && AssociatedObject.IsEffectivelyEnabled || tuple.isForced)
            .StartWith(false)
            .Do(isVisible => Target!.IsVisible = isVisible)
            .Subscribe()
            .DisposeWith(disposable);

        return disposable;
    }

    private static Control? GetMainView()
    {
        return Application.Current!.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime => classicDesktopStyleApplicationLifetime.MainWindow,
            ISingleViewApplicationLifetime singleViewApplicationLifetime => singleViewApplicationLifetime.MainView,
            _ => null
        };
    }

    private bool ContainsPoint(Visual reference, Point referencePoint, Visual toCheck)
    {
        var translatePoint = toCheck.TranslatePoint(new Point(), reference);

        if (translatePoint is not { } p)
        {
            return false;
        }

        var finalBounds = new Rect(p, toCheck.Bounds.Size).Inflate(InflateHitBoxesBy);
        return finalBounds.Contains(referencePoint);
    }
}