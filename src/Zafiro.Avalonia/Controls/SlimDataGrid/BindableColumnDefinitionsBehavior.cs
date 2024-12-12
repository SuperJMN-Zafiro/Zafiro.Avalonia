using System.Reactive.Disposables;
using Avalonia.Data;
using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.Controls.SlimDataGrid;

public class BindableColumnDefinitionsBehavior : Behavior<Grid>
{
    private CompositeDisposable? disposables;

    public static readonly StyledProperty<ColumnDefinitions> ColumnDefinitionsProperty =
        AvaloniaProperty.Register<BindableColumnDefinitionsBehavior, ColumnDefinitions>(
            nameof(ColumnDefinitions), defaultBindingMode: BindingMode.TwoWay);

    protected override void OnAttached()
    {
        base.OnAttached();

        disposables = new CompositeDisposable();
        
        this.WhenAnyValue(x => x.ColumnDefinitions)
            .WhereNotNull()
            .Subscribe(definition =>
            {
                if (AssociatedObject != null)
                {
                    AssociatedObject.ColumnDefinitions = definition;
                }
            })
            .DisposeWith(disposables);
    }

    protected override void OnDetaching()
    {
        disposables?.Dispose();
        base.OnDetaching();
    }

    public ColumnDefinitions ColumnDefinitions
    {
        get => GetValue(ColumnDefinitionsProperty);
        set => SetValue(ColumnDefinitionsProperty, value);
    }
}