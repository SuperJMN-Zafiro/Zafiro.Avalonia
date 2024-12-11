using System.Reactive.Disposables;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;

namespace Zafiro.Avalonia.Controls.DataGrid;

public class Header : TemplatedControl
{
    public static readonly StyledProperty<IDataTemplate?> ItemTemplateProperty = AvaloniaProperty.Register<Header, IDataTemplate?>(
        nameof(ItemTemplate));

    public IDataTemplate? ItemTemplate
    {
        get => GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }
    
    public Header(DataColumn column, int index)
    {
        Column = column;
        Index = index;
        this.WhenAnyValue(vm => vm.Column.HeaderTemplate)
            .WhereNotNull()
            .Subscribe(template => ItemTemplate = template)
            .DisposeWith(disposables);
        Value = column.Header;
    }

    private object? value;
    private readonly CompositeDisposable disposables = new();

    public static readonly DirectProperty<Header, object?> ValueProperty = AvaloniaProperty.RegisterDirect<Header, object?>(
        nameof(Value), o => o.Value, (o, v) => o.Value = v);

    public object? Value
    {
        get => value;
        set => SetAndRaise(ValueProperty, ref this.value, value);
    }

    protected override void OnUnloaded(RoutedEventArgs e)
    {
        disposables.Dispose();
        base.OnUnloaded(e);
    }

    public DataColumn Column { get; init; }
    public int Index { get; init; }
}