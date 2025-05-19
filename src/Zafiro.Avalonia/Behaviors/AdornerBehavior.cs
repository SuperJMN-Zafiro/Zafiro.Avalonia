﻿using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Xaml.Interactions.Custom;

namespace Zafiro.Avalonia.Behaviors;

public enum Alignment
{
    MiddleRight,
    BottomRight
}

public enum DataContextMode
{
    TemplatedParent,
    DataContext
}

public class AdornerBehavior : AttachedToVisualTreeBehavior<Control>
{
    public static readonly StyledProperty<Alignment> PlacementModeProperty = AvaloniaProperty.Register<AdornerBehavior, Alignment>(nameof(PlacementMode));

    [ResolveByName] public Control? Adorner { get; set; }

    public Alignment PlacementMode
    {
        get => GetValue(PlacementModeProperty);
        set => SetValue(PlacementModeProperty, value);
    }

    public DataContextMode AdornerDataContextMode { get; set; } = DataContextMode.TemplatedParent;

    protected override IDisposable OnAttachedToVisualTreeOverride()
    {
        var disposables = new CompositeDisposable();

        if (AssociatedObject is null)
        {
            return disposables;
        }

        var layer = AdornerLayer.GetAdornerLayer(AssociatedObject);

        if (layer is null || Adorner is null)
        {
            return disposables;
        }

        Adorner.DataContext = AdornerDataContextMode == DataContextMode.TemplatedParent ? AssociatedObject.TemplatedParent : AssociatedObject.DataContext;

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
            .Do(_ => ArrangeAdorner(Adorner, AssociatedObject, layer))
            .Subscribe()
            .DisposeWith(disposables);

        Disposable
            .Create(() => layer.Children.Remove(Adorner))
            .DisposeWith(disposables);

        ArrangeAdorner(Adorner, AssociatedObject, layer);

        return disposables;
    }

    private void ArrangeAdorner(Visual adorner, Visual adorned, Visual layer)
    {
        var translatePoint = adorned.TranslatePoint(new Point(), layer);

        if (translatePoint is not { } point)
        {
            return;
        }

        var finalBounds = new Rect(point.X, point.Y, adorned.Bounds.Width, adorned.Bounds.Height);
        AlignTo(adorner, finalBounds, PlacementMode);
    }

    private static void AlignTo(Visual target, Rect bounds, Alignment placementMode)
    {
        switch (placementMode)
        {
            case Alignment.MiddleRight:
                Canvas.SetLeft(target, bounds.Right);
                Canvas.SetTop(target, bounds.Y + (bounds.Height / 2 - target.Bounds.Height / 2));
                break;
            case Alignment.BottomRight:
                Canvas.SetLeft(target, bounds.Right);
                Canvas.SetTop(target, bounds.Y + (bounds.Height - target.Bounds.Height));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(placementMode));
        }
    }
}