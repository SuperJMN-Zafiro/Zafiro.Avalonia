using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Presenters;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactions.Custom;
using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.Behaviors;

// Tracks which children of a Panel are visible vs invisible within the current viewport.
// Viewport sources supported:
//  - ScrollViewer: uses Offset + Viewport (via ScrollContentPresenter if available)
//  - Fallback: nearest ancestor with ClipToBounds=true; otherwise the VisualRoot/TopLevel bounds
public class ChildrenVisibilityTrackerBehavior : DisposingBehavior<Panel>
{
    public static readonly StyledProperty<IReadOnlyList<Control>> VisibleChildrenProperty =
        AvaloniaProperty.Register<ChildrenVisibilityTrackerBehavior, IReadOnlyList<Control>>(
            nameof(VisibleChildren), Array.Empty<Control>());

    public static readonly StyledProperty<IReadOnlyList<Control>> InvisibleChildrenProperty =
        AvaloniaProperty.Register<ChildrenVisibilityTrackerBehavior, IReadOnlyList<Control>>(
            nameof(InvisibleChildren), Array.Empty<Control>());

    // If true, partially visible children are counted as visible; otherwise they are treated as invisible
    public static readonly StyledProperty<bool> CountPartialAsVisibleProperty =
        AvaloniaProperty.Register<ChildrenVisibilityTrackerBehavior, bool>(nameof(CountPartialAsVisible), true);

    public IReadOnlyList<Control> VisibleChildren
    {
        get => GetValue(VisibleChildrenProperty);
        set => SetValue(VisibleChildrenProperty, value);
    }

    public IReadOnlyList<Control> InvisibleChildren
    {
        get => GetValue(InvisibleChildrenProperty);
        set => SetValue(InvisibleChildrenProperty, value);
    }

    public bool CountPartialAsVisible
    {
        get => GetValue(CountPartialAsVisibleProperty);
        set => SetValue(CountPartialAsVisibleProperty, value);
    }

    protected override IDisposable OnAttachedOverride()
    {
        if (AssociatedObject is null)
        {
            return Disposable.Empty;
        }

        var disposables = new CompositeDisposable();
        var panel = AssociatedObject;

        // Discover scroll/viewport actors
        var scroller = panel.FindAncestorOfType<ScrollViewer>();
        var contentPresenter = scroller?.GetVisualDescendants().OfType<ScrollContentPresenter>().FirstOrDefault();

        // Streams that trigger recomputation
        var panelBoundsChanged = panel.GetObservable(Visual.BoundsProperty).Select(_ => Unit.Default);

        var childrenCollectionChanged = Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                h => panel.Children.CollectionChanged += h,
                h => panel.Children.CollectionChanged -= h)
            .Select(_ => Unit.Default)
            .StartWith(Unit.Default);

        // For each current set of children, observe their Bounds and IsVisible changes
        var childrenPropsChanged = childrenCollectionChanged
            .Select(_ => panel.Children.ToArray())
            .Select(children =>
            {
                if (children.Length == 0)
                {
                    return Observable.Return(Unit.Default);
                }

                var perChild = children.Select(child =>
                    Observable.Merge(
                        child.GetObservable(Visual.BoundsProperty).Select(__ => Unit.Default),
                        child.GetObservable(Visual.IsVisibleProperty).Select(__ => Unit.Default)));

                return perChild.Merge();
            })
            .Switch();

        // ScrollViewer changes
        var offsetChanged = scroller is null
            ? Observable.Empty<Unit>()
            : scroller.GetObservable(ScrollViewer.OffsetProperty).Select(_ => Unit.Default);

        var viewportChanged = scroller is null
            ? Observable.Empty<Unit>()
            : scroller.GetObservable(ScrollViewer.ViewportProperty).Select(_ => Unit.Default);

        // Ancestor viewport bounds changes (fallback path)
        var viewportOwner = GetViewportOwnerFallback(panel);
        var viewportOwnerBoundsChanged = viewportOwner is null
            ? Observable.Empty<Unit>()
            : viewportOwner.GetObservable(Visual.BoundsProperty).Select(_ => Unit.Default);

        // CountPartialAsVisible toggles should also trigger recompute
        var policyChanged = this.GetObservable(CountPartialAsVisibleProperty).Select(_ => Unit.Default);

        var recompute = Observable.Merge(
                panelBoundsChanged,
                childrenCollectionChanged,
                childrenPropsChanged,
                offsetChanged,
                viewportChanged,
                viewportOwnerBoundsChanged,
                policyChanged)
            .Throttle(TimeSpan.FromMilliseconds(16), AvaloniaScheduler.Instance) // coalesce bursts
            .ObserveOn(AvaloniaScheduler.Instance)
            .StartWith(Unit.Default)
            .Subscribe(_ => UpdateLists(panel, scroller, contentPresenter, viewportOwner));

        recompute.DisposeWith(disposables);

        return disposables;
    }

    private static Control? GetViewportOwnerFallback(Panel panel)
    {
        var clipAncestor = panel.GetVisualAncestors().OfType<Control>().FirstOrDefault(c => c.ClipToBounds);
        if (clipAncestor != null)
        {
            return clipAncestor;
        }

        var root = panel.GetVisualRoot() as Control;
        return root;
    }

    private void UpdateLists(Panel panel, ScrollViewer? scroller, ScrollContentPresenter? contentPresenter, Control? fallbackViewportOwner)
    {
        if (panel is null)
        {
            VisibleChildren = Array.Empty<Control>();
            InvisibleChildren = Array.Empty<Control>();
            return;
        }

        Visual? target; // coordinate space to project children into
        Rect viewportRect;

        if (scroller != null && contentPresenter != null)
        {
            // Viewport in content coordinates
            target = contentPresenter;
            viewportRect = new Rect(new Point(scroller.Offset.X, scroller.Offset.Y), scroller.Viewport);
            System.Diagnostics.Debug.WriteLine($"Using ScrollViewer viewport: {viewportRect}");
        }
        else if (fallbackViewportOwner != null)
        {
            target = fallbackViewportOwner;
            viewportRect = new Rect(default, fallbackViewportOwner.Bounds.Size);
            System.Diagnostics.Debug.WriteLine($"Using fallback viewport owner ({fallbackViewportOwner.GetType().Name}): {viewportRect}");
        }
        else
        {
            target = panel.GetVisualRoot() as Visual;
            var size = target is TopLevel tl ? tl.ClientSize : new Size(0, 0);
            viewportRect = new Rect(default, size);
            System.Diagnostics.Debug.WriteLine($"Using VisualRoot viewport: {viewportRect}");
        }

        var visible = new List<Control>();
        var invisible = new List<Control>();
        var totalChildren = panel.Children.OfType<Control>().Count();

        System.Diagnostics.Debug.WriteLine($"\n=== UpdateLists: {totalChildren} total children, viewport: {viewportRect} ===");

        foreach (var child in panel.Children.OfType<Control>())
        {
            var childIndex = panel.Children.IndexOf(child);
            
            // If child itself is not visible, mark as invisible
            if (!child.IsVisible)
            {
                invisible.Add(child);
                System.Diagnostics.Debug.WriteLine($"Child {childIndex}: NOT VISIBLE (IsVisible=false)");
                continue;
            }

            var childRect = ProjectBounds(child, target);
            if (childRect is null || childRect.Value.Width <= 0 || childRect.Value.Height <= 0)
            {
                invisible.Add(child);
                System.Diagnostics.Debug.WriteLine($"Child {childIndex}: INVALID BOUNDS ({childRect})");
                continue;
            }

            var intersection = viewportRect.Intersect(childRect.Value);
            var intersectionArea = intersection.Width * intersection.Height;
            var childArea = childRect.Value.Width * childRect.Value.Height;
            
            var isVisible = CountPartialAsVisible
                ? intersection.Width > 0 && intersection.Height > 0
                : NearlyEquals(intersectionArea, childArea);

            System.Diagnostics.Debug.WriteLine($"Child {childIndex}: bounds={childRect.Value}, intersection={intersection}, visible={isVisible}");

            if (isVisible)
            {
                visible.Add(child);
            }
            else
            {
                invisible.Add(child);
            }
        }

        System.Diagnostics.Debug.WriteLine($"RESULT: {visible.Count} visible, {invisible.Count} invisible\n");

        VisibleChildren = visible;
        InvisibleChildren = invisible;
    }

    private static Rect? ProjectBounds(Control child, Visual? target)
    {
        if (target is null)
        {
            return null;
        }

        var w = child.Bounds.Width;
        var h = child.Bounds.Height;
        var tl = child.TranslatePoint(new Point(0, 0), target);
        var br = child.TranslatePoint(new Point(w, h), target);

        if (tl is null || br is null)
        {
            return null;
        }

        return new Rect(tl.Value, br.Value);
    }

    private static bool NearlyEquals(double a, double b, double epsilon = 0.5)
    {
        return Math.Abs(a - b) < epsilon;
    }
}