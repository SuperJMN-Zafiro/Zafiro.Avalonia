using Avalonia.Controls;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Avalonia;
using ReactiveUI;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia;

public class WrapToColumnHeaderBehavior : Behavior<TreeDataGrid>
{
    public static readonly DirectProperty<WrapToColumnHeaderBehavior, Rect> BoundsProperty =
        AvaloniaProperty.RegisterDirect<WrapToColumnHeaderBehavior, Rect>(
            "Offset", o => o.Bounds, (o, v) => o.Bounds = v);

    private readonly CompositeDisposable disposable = new();

    private Rect bounds;

    public Rect Bounds
    {
        get => bounds;
        set => SetAndRaise(BoundsProperty, ref bounds, value);
    }

    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is null)
        {
            return;
        }

        this.WhenAnyValue(x => x.Control, x => x.Index)
            .Where(tuple => tuple.Item1 is Control)
            .Select(tuple => GetBounds(AssociatedObject, tuple.Item2).Select(rect => new { tuple.Item1, tuple.Item2, rect }))
            .Switch()
            .Do(obj => Wrap((Control)obj.Item1, obj.rect))
            .Subscribe()
            .DisposeWith(disposable);
    }

    protected override void OnDetachedFromVisualTree()
    {
        disposable.Dispose();
        base.OnDetachedFromVisualTree();
    }

    private void Wrap(Control control, Rect rect)
    {
        if (SetWidth)
        {
            control.Width = rect.Width;
        }

        if (SetHeight)
        {
            control.Height = rect.Height;
        }

        control.Margin = new Thickness(rect.Left, rect.Top, 0, 0);
    }

    private IObservable<Rect> GetBounds(TreeDataGrid control, int index)
    {
        return Observable.Interval(TimeSpan.FromMilliseconds(100), RxApp.MainThreadScheduler)
            .Select(_ => GetColumnHeaders(control))
            .Where(x => x.Any())
            .Select(headers => headers.Skip(index).Take(1).FirstOrDefault())
            .WhereNotNull()
            .Select(header => header.Bounds);
    }

    private IEnumerable<TreeDataGridColumnHeader> GetColumnHeaders(IVisual treeDataGrid)
    {
        var treeDataGridColumnHeaders = treeDataGrid.GetVisualDescendants().OfType<TreeDataGridColumnHeader>();
        return treeDataGridColumnHeaders;
    }

    private int index;

    public static readonly DirectProperty<WrapToColumnHeaderBehavior, int> IndexProperty = AvaloniaProperty.RegisterDirect<WrapToColumnHeaderBehavior, int>(
        "Index", o => o.Index, (o, v) => o.Index = v);

    public int Index
    {
        get => index;
        set => SetAndRaise(IndexProperty, ref index, value);
    }

    private object _control;

    public static readonly DirectProperty<WrapToColumnHeaderBehavior, object> ControlProperty = AvaloniaProperty.RegisterDirect<WrapToColumnHeaderBehavior, object>(
        "Control", o => o.Control, (o, v) => o.Control = v);

    [ResolveByName]
    public object Control
    {
        get => _control;
        set => SetAndRaise(ControlProperty, ref _control, value);
    }

    private bool setHeight;

    public static readonly DirectProperty<WrapToColumnHeaderBehavior, bool> SetHeightProperty = AvaloniaProperty.RegisterDirect<WrapToColumnHeaderBehavior, bool>(
        "SetHeight", o => o.SetHeight, (o, v) => o.SetHeight = v);

    public bool SetHeight
    {
        get => setHeight;
        set => SetAndRaise(SetHeightProperty, ref setHeight, value);
    }

    private bool setWidth;

    public static readonly DirectProperty<WrapToColumnHeaderBehavior, bool> SetWidthProperty = AvaloniaProperty.RegisterDirect<WrapToColumnHeaderBehavior, bool>(
        "SetWidth", o => o.SetWidth, (o, v) => o.SetWidth = v);

    public bool SetWidth
    {
        get => setWidth;
        set => SetAndRaise(SetWidthProperty, ref setWidth, value);
    }
}