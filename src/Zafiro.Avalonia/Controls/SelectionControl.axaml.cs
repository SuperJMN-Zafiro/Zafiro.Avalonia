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
            .Select(x => x.Kind().Select(kind => (kind, x)))
            .Switch()
            .Select(tuple => tuple.kind == SelectionKind.None ? tuple.x.SelectAll : tuple.x.SelectNone)
            .ToProperty(this, CycleSelectionProperty)
            .DisposeWith(disposables);
        
        this
            .WhenAnyValue(x => x.Selection)
            .WhereNotNull()
            .Select(x => x.Kind())
            .Switch()
            .ToProperty(this, SelectionKindProperty)
            .DisposeWith(disposables);
        
        this
            .WhenAnyValue(x => x.Selection)
            .WhereNotNull()
            .Select(x => x.SelectionCount)
            .Switch()
            .ToProperty(this, SelectedCountProperty)
            .DisposeWith(disposables);
        
        this
            .WhenAnyValue(x => x.Selection)
            .WhereNotNull()
            .Select(x => x.TotalCount)
            .Switch()
            .ToProperty(this, TotalCountProperty)
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

    private int _selectedCount;

    public static readonly DirectProperty<SelectionControl, int> SelectedCountProperty = AvaloniaProperty.RegisterDirect<SelectionControl, int>(
        "SelectedCount", o => o.SelectedCount, (o, v) => o.SelectedCount = v);

    public int SelectedCount
    {
        get => _selectedCount;
        set => SetAndRaise(SelectedCountProperty, ref _selectedCount, value);
    }

    private int _totalCount;

    public static readonly DirectProperty<SelectionControl, int> TotalCountProperty = AvaloniaProperty.RegisterDirect<SelectionControl, int>(
        "TotalCount", o => o.TotalCount, (o, v) => o.TotalCount = v);

    public int TotalCount
    {
        get => _totalCount;
        set => SetAndRaise(TotalCountProperty, ref _totalCount, value);
    }
}