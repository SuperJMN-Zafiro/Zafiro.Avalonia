using System.ComponentModel;
using System.Reactive.Disposables;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.ViewLocators;

namespace Zafiro.Avalonia.Behaviors;

public class SortDataGridColumnBehavior : Behavior<DataGrid>
{
    public static readonly StyledProperty<string> ColumnNameProperty = AvaloniaProperty.Register<SortDataGridColumnBehavior, string>(
        nameof(ColumnName));

    public static readonly StyledProperty<ListSortDirection> SortDirectionProperty = AvaloniaProperty.Register<SortDataGridColumnBehavior, ListSortDirection>(
        nameof(SortDirection));

    public static readonly StyledProperty<int> DisplayIndexProperty = AvaloniaProperty.Register<SortDataGridColumnBehavior, int>(
        nameof(DisplayIndex), -1);

    private readonly CompositeDisposable disposable = new();

    public string ColumnName
    {
        get => GetValue(ColumnNameProperty);
        set => SetValue(ColumnNameProperty, value);
    }

    public ListSortDirection SortDirection
    {
        get => GetValue(SortDirectionProperty);
        set => SetValue(SortDirectionProperty, value);
    }

    public int DisplayIndex
    {
        get => GetValue(DisplayIndexProperty);
        set => SetValue(DisplayIndexProperty, value);
    }

    protected override void OnAttached()
    {
        AssociatedObject?.OnEvent(Control.LoadedEvent, RoutingStrategies.Direct)
            .Do(_ => Sort())
            .Subscribe()
            .DisposeWith(disposable);
    }

    protected override void OnDetaching()
    {
        disposable.Dispose();
        base.OnDetaching();
    }

    private void Sort()
    {
        var byColumnName = AssociatedObject.AsMaybe()
            .Bind(x => x.Columns.TryFirst(column => Equals(column.Header, ColumnName)));

        var byIndex = AssociatedObject.AsMaybe()
            .Bind(x => x.Columns.TryFirst(column => Equals(column.DisplayIndex, DisplayIndex)));

        var column = byColumnName.Or(byIndex);
        column.Execute(gridColumn => gridColumn.Sort(ListSortDirection.Descending));
    }
}