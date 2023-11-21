using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactions.Custom;

namespace Zafiro.Avalonia.Behaviors;

public class AdornerBehavior : AttachedToVisualTreeBehavior<Control>
{
    public Control? Adorner { get; set; }

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

        Disposable.Create(() => layer.Children.Remove(Adorner))
            .DisposeWith(disposables);

        ArrangeAdorner(AssociatedObject, layer);
    }

    private void ArrangeAdorner(Visual adorned, Visual layer)
    {
        var point = adorned.TranslatePoint(new Point(), layer);

        if (!point.HasValue || AssociatedObject is null)
        {
            return;
        }

        var p = point.Value;

        var finalBounds = new Rect(p.X, p.Y, AssociatedObject.Bounds.Width, AssociatedObject.Bounds.Height);
        Canvas.SetLeft(Adorner!, finalBounds.Right);
        Canvas.SetTop(Adorner!, finalBounds.Y + (finalBounds.Height / 2 - Adorner!.Bounds.Height / 2));
    }
}