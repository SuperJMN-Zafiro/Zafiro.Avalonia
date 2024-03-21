using System.Reactive.Disposables;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Zafiro.Avalonia.Controls;

public static class AvaloniaObjectMixin
{
    public static IDisposable ToProperty<T>(this IObservable<T> observable, AvaloniaObject avaloniaObject, AvaloniaProperty<T> property)
    {
        return avaloniaObject.Bind(property, observable);
    }
}

public class SelectionControl : TemplatedControl
{
    public static readonly StyledProperty<ISelectionHandler> SelectionProperty = AvaloniaProperty.Register<SelectionControl, ISelectionHandler>(
        "Selection");

    public SelectionControl()
    {
        this
            .WhenAnyValue(x => x.Selection)
            .WhereNotNull()
            .Select(x => x.SelectionKind.Select(kind => (kind, x)))
            .Switch()
            .Select(tuple => tuple.kind == SelectionKind.None ? tuple.x.SelectAll : tuple.x.SelectNone)
            .ToProperty(this, CycleSelectionProperty)
            .DisposeWith(disposables);
        
        this
            .WhenAnyValue(x => x.Selection)
            .WhereNotNull()
            .Select(x => x.SelectionKind)
            .Switch()
            .Select(kind => kind)
            .ToProperty(this, SelectionKindProperty)
            .DisposeWith(disposables);
    }

    private SelectionKind _selectionKind;

    public static readonly DirectProperty<SelectionControl, SelectionKind> SelectionKindProperty = AvaloniaProperty.RegisterDirect<SelectionControl, SelectionKind>(
        "SelectionKind", o => o.SelectionKind, (o, v) => o.SelectionKind = v);

    public SelectionKind SelectionKind
    {
        get => _selectionKind;
        set => SetAndRaise(SelectionKindProperty, ref _selectionKind, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposables.Dispose();
        base.OnUnloaded(e);
    }

    public ISelectionHandler Selection
    {
        get => GetValue(SelectionProperty);
        set => SetValue(SelectionProperty, value);
    }

    private ICommand _cycleSelection;

    public static readonly DirectProperty<SelectionControl, ICommand> CycleSelectionProperty = AvaloniaProperty.RegisterDirect<SelectionControl, ICommand>(
        "CycleSelection", o => o.CycleSelection, (o, v) => o.CycleSelection = v);

    private readonly CompositeDisposable disposables = new();

    public ICommand CycleSelection
    {
        get => _cycleSelection;
        private set => SetAndRaise(CycleSelectionProperty, ref _cycleSelection, value);
    }
}