using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Selection;
using DynamicData;

namespace Zafiro.Avalonia.Controls;

public class SelectionControl : TemplatedControl
{
    private ICommand clear;

    private ICommand selectAll;

    public static readonly StyledProperty<ISelectionModel> SelectionProperty = AvaloniaProperty.Register<SelectionControl, ISelectionModel>(
        nameof(Selection));

    public static readonly DirectProperty<SelectionControl, ICommand> SelectAllProperty = AvaloniaProperty.RegisterDirect<SelectionControl, ICommand>(
        "SelectAll", o => o.SelectAll, (o, v) => o.SelectAll = v);

    public static readonly DirectProperty<SelectionControl, ICommand> ClearProperty = AvaloniaProperty.RegisterDirect<SelectionControl, ICommand>(
        "Clear", o => o.Clear, (o, v) => o.Clear = v);

    public SelectionControl()
    {
        var hasItems = this.WhenAnyValue(x => x.Selection)
            .WhereNotNull()
            .Select(model => Observable.FromEventPattern(model, nameof(ISelectionModel.SelectionChanged)))
            .Switch()
            .Throttle(TimeSpan.FromSeconds(0.1), AvaloniaScheduler.Instance)
            .Select(_ => Selection.SelectedItems.Any());

        var selectedItems = this.WhenAnyValue(x => x.Selection)
            .WhereNotNull()
            .Select(model => Observable.FromEventPattern(model, nameof(ISelectionModel.SelectionChanged)))
            .Switch()
            .Throttle(TimeSpan.FromSeconds(0.1), AvaloniaScheduler.Instance)
            .Select(_ => (SelectionCount: Selection.Count, SourceCount: Selection.Source!.Cast<object>().Count()));
        
        clear = ReactiveCommand.Create(() => Selection.Clear(), selectedItems.Select(i => i.SelectionCount > 0));
        selectAll = ReactiveCommand.Create(() => selectedItems.Select(i => i.SelectionCount != i.SourceCount && i.SelectionCount > 0));
    }

    public ISelectionModel Selection
    {
        get => GetValue(SelectionProperty);
        set => SetValue(SelectionProperty, value);
    }

    public ICommand SelectAll
    {
        get => selectAll;
        private set => SetAndRaise(SelectAllProperty, ref selectAll, value);
    }

    public ICommand Clear
    {
        get => clear;
        private set => SetAndRaise(ClearProperty, ref clear, value);
    }
}