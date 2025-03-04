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

    public ColumnDefinitions ColumnDefinitions
    {
        get => GetValue(ColumnDefinitionsProperty);
        set => SetValue(ColumnDefinitionsProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();

        disposables = new CompositeDisposable();

        this.WhenAnyValue(x => x.ColumnDefinitions)
            .WhereNotNull()
            .Subscribe(columnDefinitions =>
            {
                if (AssociatedObject != null && IsEnabled)
                {
                    AssociatedObject.ColumnDefinitions.Clear();

                    AssociatedObject.ColumnDefinitions.AddRange(columnDefinitions);
                }
            })
            .DisposeWith(disposables);
    }

    protected override void OnDetaching()
    {
        disposables?.Dispose();
        base.OnDetaching();
    }
}