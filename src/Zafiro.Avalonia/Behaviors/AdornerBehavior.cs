using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Xaml.Interactions.Custom;

namespace Zafiro.Avalonia.Behaviors;

public enum Alignment
{
    MiddleRight,
    BottomRight
}

public class AdornerBehavior : AttachedToVisualTreeBehavior<Control>
{
    public static readonly StyledProperty<Alignment> PlacementModeProperty = AvaloniaProperty.Register<AdornerBehavior, Alignment>(nameof(PlacementMode));
    public Control? Adorner { get; set; }

    public Alignment PlacementMode
    {
        get => GetValue(PlacementModeProperty);
        set => SetValue(PlacementModeProperty, value);
    }

    protected override void OnAttachedToVisualTree(CompositeDisposable disposables)
    {
        if (AssociatedObject is null)
        {
            return;
        }

        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

        if (layer is null || Adorner is null)
        {
            return;
        }

        Adorner.DataContext = AssociatedObject.TemplatedParent;

        layer.Children.Add(Adorner);

        Observable.FromEventPattern(
                handler => AssociatedObject.LayoutUpdated += handler,
                handler =>
                {
                    if (AssociatedObject != null)
                    {
                        AssociatedObject.LayoutUpdated -= handler;
                    }
                })
            .Do(_ => ArrangeAdorner(AssociatedObject, layer))
            .Subscribe()
            .DisposeWith(disposables);

        Disposable
            .Create(() => layer.Children.Remove(Adorner))
            .DisposeWith(disposables);

        ArrangeAdorner(AssociatedObject, layer);
    }

    private void ArrangeAdorner(Visual adorned, Visual layer)
    {
        var translatePoint = adorned.TranslatePoint(new Point(), layer);

        if (translatePoint is not { } point)
        {
            return;
        }

        var finalBounds = new Rect(point.X, point.Y, adorned.Bounds.Width, adorned.Bounds.Height);
        AlignTo(finalBounds);
    }

    private void AlignTo(Rect finalBounds)
    {
        switch (PlacementMode)
        {
            case Alignment.MiddleRight:
                Canvas.SetLeft(Adorner!, finalBounds.Right);
                Canvas.SetTop(Adorner!, finalBounds.Y + (finalBounds.Height / 2 - Adorner!.Bounds.Height / 2));
                break;
            case Alignment.BottomRight:
                Canvas.SetLeft(Adorner!, finalBounds.Right);
                Canvas.SetTop(Adorner!, finalBounds.Y + (finalBounds.Height - Adorner!.Bounds.Height));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}