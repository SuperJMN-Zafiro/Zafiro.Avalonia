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

    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
    
    private object? value;
    private readonly CompositeDisposable compositeDisposable = new();

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

        this.WhenAnyValue(x => x.Column.CellTemplate)
            .Subscribe(template => this.ItemTemplate = template)
            .DisposeWith(compositeDisposable);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        compositeDisposable.Dispose();
        base.OnUnloaded(e);
    }

    public object Data { get; }
    public Column Column { get; }
    public int Index { get; }
    
    public object? Value
    {
        get => value;
        set => SetAndRaise(ValueProperty, ref this.value, value);
    }
}