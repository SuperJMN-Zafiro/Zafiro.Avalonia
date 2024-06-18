using System.Reactive.Disposables;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;

namespace Zafiro.Avalonia.Controls;

public class SelectionControl : TemplatedControl
{
    private ICommand clearSelectionCommand;

    private ICommand selectAllCommand;

    private int selectedCount;

    private SelectionKind selectionKind;

    private int totalCount;

    private readonly CompositeDisposable disposables = new();

    public static readonly StyledProperty<ISelectionHandler> SelectionProperty = AvaloniaProperty.Register<SelectionControl, ISelectionHandler>(
        "Selection");

    public static readonly DirectProperty<SelectionControl, ICommand> SelectAllCommandProperty = AvaloniaProperty.RegisterDirect<SelectionControl, ICommand>(
        "SelectAllCommand", o => o.SelectAllCommand, (o, v) => o.SelectAllCommand = v);

    public static readonly DirectProperty<SelectionControl, SelectionKind> SelectionKindProperty = AvaloniaProperty.RegisterDirect<SelectionControl, SelectionKind>(
        "SelectionKind", o => o.SelectionKind, (o, v) => o.SelectionKind = v);

    public static readonly DirectProperty<SelectionControl, ICommand> ClearSelectionCommandProperty = AvaloniaProperty.RegisterDirect<SelectionControl, ICommand>(
        "ClearSelectionCommand", o => o.ClearSelectionCommand, (o, v) => o.ClearSelectionCommand = v);

    public static readonly DirectProperty<SelectionControl, int> SelectedCountProperty = AvaloniaProperty.RegisterDirect<SelectionControl, int>(
        "SelectedCount", o => o.SelectedCount, (o, v) => o.SelectedCount = v);

    public static readonly DirectProperty<SelectionControl, int> TotalCountProperty = AvaloniaProperty.RegisterDirect<SelectionControl, int>(
        "TotalCount", o => o.TotalCount, (o, v) => o.TotalCount = v);

    public SelectionControl()
    {
        this
            .WhenAnyValue(x => x.Selection.SelectAll)
            .ToProperty(this, SelectAllCommandProperty)
            .DisposeWith(disposables);

        this
            .WhenAnyValue(x => x.Selection.SelectNone)
            .ToProperty(this, ClearSelectionCommandProperty)
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

    public ICommand SelectAllCommand
    {
        get => selectAllCommand;
        set => SetAndRaise(SelectAllCommandProperty, ref selectAllCommand, value);
    }

    public SelectionKind SelectionKind
    {
        get => selectionKind;
        set => SetAndRaise(SelectionKindProperty, ref selectionKind, value);
    }

    public ICommand ClearSelectionCommand
    {
        get => clearSelectionCommand;
        set => SetAndRaise(ClearSelectionCommandProperty, ref clearSelectionCommand, value);
    }

    public ISelectionHandler Selection
    {
        get => GetValue(SelectionProperty);
        set => SetValue(SelectionProperty, value);
    }

    public int SelectedCount
    {
        get => selectedCount;
        set => SetAndRaise(SelectedCountProperty, ref selectedCount, value);
    }

    public int TotalCount
    {
        get => totalCount;
        set => SetAndRaise(TotalCountProperty, ref totalCount, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposables.Dispose();
        base.OnUnloaded(e);
    }
}