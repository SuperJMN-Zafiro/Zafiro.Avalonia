using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;

namespace Zafiro.Avalonia.Controls.SlimDataGrid;

public class Cell : TemplatedControl
{
    public static readonly DirectProperty<Cell, object?> ValueProperty = AvaloniaProperty.RegisterDirect<Cell, object?>(
        nameof(Value), o => o.Value, (o, v) => o.Value = v);


    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<Cell, IDataTemplate?>(
        nameof(ItemTemplate));

    private readonly CompositeDisposable compositeDisposable = new();

    private object? value;

    public Cell(object data, Column column, int index)
    {
        Data = data;
        Column = column;
        Index = index;

        var binding = column.Binding;

        if (binding != null)
        {
            DataContext = data;
            Bind(ValueProperty, binding);
        }
        else
        {
            // Default fallback: use the row item as both DataContext and Value
            // so that CellTemplate bindings like {Binding Prop} work out of the box
            // and, without a template, ContentControl can display the item's ToString().
            DataContext = data;
            Value = data;
        }

        this.WhenAnyValue(x => x.Column.CellTemplate)
            .Subscribe(template => this.ItemTemplate = template)
            .DisposeWith(compositeDisposable);
    }

    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    public object Data { get; }
    public Column Column { get; }
    public int Index { get; }

    public object? Value
    {
        get => value;
        set => SetAndRaise(ValueProperty, ref this.value, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        compositeDisposable.Dispose();
        base.OnUnloaded(e);
    }
}