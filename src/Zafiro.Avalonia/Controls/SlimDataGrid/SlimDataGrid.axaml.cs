using System.Collections;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using Avalonia.Styling;
using DynamicData;
using DynamicData.Binding;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.SlimDataGrid;

public class SlimDataGrid : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable?> ItemsSourceProperty = AvaloniaProperty.Register<SlimDataGrid, IEnumerable?>(
        nameof(ItemsSource));

    public static readonly DirectProperty<SlimDataGrid, IEnumerable<Row>?> DataRowsProperty = AvaloniaProperty.RegisterDirect<SlimDataGrid, IEnumerable<Row>?>(
        nameof(DataRows), o => o.DataRows, (o, v) => o.DataRows = v);

    public static readonly DirectProperty<SlimDataGrid, ColumnDefinitions?> ColumnDefinitionsProperty = AvaloniaProperty.RegisterDirect<SlimDataGrid, ColumnDefinitions?>(
        nameof(ColumnDefinitions), o => o.ColumnDefinitions, (o, v) => o.ColumnDefinitions = v);

    public static readonly DirectProperty<SlimDataGrid, IEnumerable<Header>?> HeadersProperty = AvaloniaProperty.RegisterDirect<SlimDataGrid, IEnumerable<Header>?>(
        nameof(Headers), o => o.Headers, (o, v) => o.Headers = v);

    public static readonly StyledProperty<Thickness> HeaderBorderThicknessProperty = AvaloniaProperty.Register<SlimDataGrid, Thickness>(
        nameof(HeaderBorderThickness));

    public static readonly StyledProperty<IBrush?> HeaderBorderBrushProperty = AvaloniaProperty.Register<SlimDataGrid, IBrush?>(
        nameof(HeaderBorderBrush));

    public static readonly StyledProperty<IBrush?> HeaderBackgroundProperty = AvaloniaProperty.Register<SlimDataGrid, IBrush?>(
        nameof(HeaderBackground));

    public static readonly StyledProperty<ControlTheme?> RowThemeProperty = AvaloniaProperty.Register<SlimDataGrid, ControlTheme?>(
        nameof(RowTheme));

    public static readonly StyledProperty<Thickness> HeaderPaddingProperty = AvaloniaProperty.Register<SlimDataGrid, Thickness>(
        nameof(HeaderPadding));

    private ColumnDefinitions? columnDefinitions;

    private IEnumerable<Row>? dataRows;

    private IEnumerable<Header>? headers;

    public SlimDataGrid()
    {
        // Actualizamos Headers cuando cambien las columnas.
        this.WhenAnyValue(x => x.Columns)
            .WhereNotNull()
            .Select(cols => cols.Select((col, index) => new Header(col, index)))
            .Subscribe(h => Headers = h);

        // Convertimos el ItemsSource a un ChangeSet, lo transformamos y lo bindemos.
        this.WhenAnyValue(x => x.ItemsSource)
            .WhereNotNull()
            .Select(source => source.Cast<object>()
                .ToObservableChangeSetIfPossible<object>()
                .Transform(item => new Row(item, Columns) { Theme = RowTheme }))
            .Switch()
            .Bind(out var rows)
            .Subscribe(_ => DataRows = rows);
    }

    public IEnumerable? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public IEnumerable<Row>? DataRows
    {
        get => dataRows;
        private set => SetAndRaise(DataRowsProperty, ref dataRows, value);
    }

    public ColumnDefinitions? ColumnDefinitions
    {
        get => columnDefinitions;
        private set => SetAndRaise(ColumnDefinitionsProperty, ref columnDefinitions, value);
    }

    public Columns Columns { get; } = new();

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

    private IEnumerable<Row> GetRows(IList<object> items, Columns columns)
    {
        return items.Select(o => new Row(o, columns)
        {
            Theme = RowTheme
        });
    }

    private static IEnumerable<Header> GetHeaders(Columns columns)
    {
        var cols = columns;

        return cols.Select((o, i) => new Header(o, i));
    }
}