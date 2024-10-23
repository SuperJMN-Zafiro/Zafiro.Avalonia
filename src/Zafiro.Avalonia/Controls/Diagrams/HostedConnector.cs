using System.Linq.Expressions;
using System.Reactive.Disposables;
using Avalonia.Layout;
using Avalonia.Media;
using Zafiro.Avalonia.Controls.Diagrams.Drawing;
using Zafiro.Avalonia.Controls.Diagrams.Drawing.LineStrategies;

namespace Zafiro.Avalonia.Controls.Diagrams;

public class HostedConnector : Control
{
    public static readonly StyledProperty<ItemsControl?> HostProperty =
        AvaloniaProperty.Register<HostedConnector, ItemsControl?>(
            nameof(Host));

    public static readonly StyledProperty<object?> FromProperty = AvaloniaProperty.Register<HostedConnector, object?>(
        nameof(From));

    public static readonly StyledProperty<object?> ToProperty = AvaloniaProperty.Register<HostedConnector, object?>(
        nameof(To));

    private readonly CompositeDisposable disposables = new();

    public HostedConnector()
    {
        ContainerFromItem(x => x.From).Select(CanvasPositionChanged).Switch()
            .Do(_ => InvalidateVisual())
            .Subscribe()
            .DisposeWith(disposables);

        ContainerFromItem(x => x.To).Select(CanvasPositionChanged).Switch()
            .Do(_ => InvalidateVisual())
            .Subscribe()
            .DisposeWith(disposables);
    }

    public ItemsControl? Host
    {
        get => GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }

    public object? From
    {
        get => GetValue(FromProperty);
        set => SetValue(FromProperty, value);
    }

    public object? To
    {
        get => GetValue(ToProperty);
        set => SetValue(ToProperty, value);
    }

    private static IObservable<Point> CanvasPositionChanged(Control control)
    {
        var left = control.GetObservable(Canvas.LeftProperty);
        var top = control.GetObservable(Canvas.TopProperty);
        return left.CombineLatest(top).Select(tuple => new Point(tuple.First, tuple.Second));
    }

    private IObservable<Control> ContainerFromItem(Expression<Func<HostedConnector, object?>> from)
    {
        return this
            .WhenAnyValue(x => x.Host, from, (h, f) => f != null ? h?.ContainerFromItem(f) : null)
            .WhereNotNull();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (From == null || To == null || Host == null) return;

        var fromContainer = Host.ContainerFromItem(From);
        var toContainer = Host.ContainerFromItem(To);

        if (fromContainer == null || toContainer == null) return;

        var pen = new Pen(Brushes.Black, 2);
        context.Connect(this, fromContainer, toContainer, VerticalAlignment.Center, HorizontalAlignment.Right,
            VerticalAlignment.Center, HorizontalAlignment.Left, SLineStrategy.Instance, pen);
    }
}