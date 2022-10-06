using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;
using Zafiro.Avalonia;

public class ScrollToSelectedItemBehavior : Behavior<TreeDataGrid>
{
    private readonly CompositeDisposable disposables = new();

    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is { SelectionInteraction: { } selection, RowSelection: { } rowSelection })
        {
            Observable.FromEventPattern(selection, nameof(selection.SelectionChanged))
                .Select(_ => rowSelection.SelectedItem)
                .WhereNotNull()
                .Throttle(TimeSpan.FromMilliseconds(100), Scheduler.CurrentThread)
                .Do(model => AssociatedObject.BringIntoView(model))
                .Subscribe()
                .DisposeWith(disposables);
        }
    }

    protected override void OnDetachedFromVisualTree()
    {
        disposables.Dispose();
    }
}