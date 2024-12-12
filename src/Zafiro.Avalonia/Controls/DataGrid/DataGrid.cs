using System.Collections;
using System.Collections.ObjectModel;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using DynamicData.Binding;

namespace Zafiro.Avalonia.Controls.DataGrid;

public class DataGrid : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<DataGrid, IEnumerable?>(
        nameof(ItemsSource));

    public static readonly DirectProperty<DataGrid, IEnumerable<DataRow>?> DataRowsProperty = AvaloniaProperty.RegisterDirect<DataGrid, IEnumerable<DataRow>?>(
        nameof(DataRows), o => o.DataRows, (o, v) => o.DataRows = v);

    public static readonly DirectProperty<DataGrid, ColumnDefinitions?> ColumnDefinitionsProperty = AvaloniaProperty.RegisterDirect<DataGrid, ColumnDefinitions?>(
        nameof(ColumnDefinitions), o => o.ColumnDefinitions, (o, v) => o.ColumnDefinitions = v);

    public static readonly DirectProperty<DataGrid, IEnumerable<Header>?> HeadersProperty = AvaloniaProperty.RegisterDirect<DataGrid, IEnumerable<Header>?>(
        nameof(Headers), o => o.Headers, (o, v) => o.Headers = v);

    public static readonly StyledProperty<Thickness> HeaderBorderThicknessProperty = AvaloniaProperty.Register<DataGrid, Thickness>(
        nameof(HeaderBorderThickness));

    public static readonly StyledProperty<IBrush?> HeaderBorderBrushProperty = AvaloniaProperty.Register<DataGrid, IBrush?>(
        nameof(HeaderBorderBrush));

    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty = AvaloniaProperty.Register<DataGrid, IBrush?>(
        nameof(HeaderBackground));

    public static readonly StyledProperty<ControlTheme?> RowThemeProperty = AvaloniaProperty.Register<DataGrid, ControlTheme?>(
        nameof(RowTheme));

    public static readonly StyledProperty<Thickness> HeaderPaddingProperty = AvaloniaProperty.Register<DataGrid, Thickness>(
        nameof(HeaderPadding));

    private ColumnDefinitions? columnDefinitions;

    private IEnumerable<DataRow>? dataRows;

    private IEnumerable<Header>? headers;

    public DataGrid()
    {
        var changes = DataColumns.ObserveCollectionChanges()
            .Select(_ => DataColumns);
        
        this.WhenAnyValue(x => x.DataColumns)
            .WhereNotNull()
            .Select(columns => GetHeaders(columns))
            .Subscribe(h => Headers = h);

        this.WhenAnyValue(x => x.ItemsSource).WhereNotNull()
            .CombineLatest(changes, (items, dataColumns) => (items, dataColumns))
            .Select(tuple => GetRows(tuple.items.Cast<object>().ToList(), tuple.dataColumns))
            .Subscribe(rows => DataRows = rows);
    }

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IEnumerable<DataRow>? DataRows
    {
        get => dataRows;
        private set => SetAndRaise(DataRowsProperty, ref dataRows, value);
    }

    public ColumnDefinitions? ColumnDefinitions
    {
        get => columnDefinitions;
        private set => SetAndRaise(ColumnDefinitionsProperty, ref columnDefinitions, value);
    }

    public ObservableCollection<DataColumn> DataColumns { get; } = new();

    public IEnumerable<Header>? Headers
    {
        get => headers;
        private set => SetAndRaise(HeadersProperty, ref headers, value);
    }

    public Thickness HeaderBorderThickness
    {
        get => GetValue(HeaderBorderThicknessProperty);
        set => SetValue(HeaderBorderThicknessProperty, value);
    }

    public IBrush? HeaderBorderBrush
    {
        get => GetValue(HeaderBorderBrushProperty);
        set => SetValue(HeaderBorderBrushProperty, value);
    }

    public IBrush? HeaderBackground
    {
        get => GetValue(HeaderBackgroundProperty);
        set => SetValue(HeaderBackgroundProperty, value);
    }

    public ControlTheme? RowTheme
    {
        get => GetValue(RowThemeProperty);
        set => SetValue(RowThemeProperty, value);
    }

    public Thickness HeaderPadding
    {
        get => GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }

    private IEnumerable<DataRow> GetRows(IList<object> items, IEnumerable<DataColumn> columns)
    {
        return items.Select(o => new DataRow(o, columns)
        {
            Theme = RowTheme
        });
    }

    private static IEnumerable<Header> GetHeaders(IEnumerable<DataColumn> columns)
    {
        var cols = columns;

        return cols.Select((o, i) => new Header(o, i));
    }
}