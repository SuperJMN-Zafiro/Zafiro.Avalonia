using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using ReactiveUI;
using Zafiro.Avalonia;

public class ScrollToSelectedItemBehavior<T> : Behavior<TreeDataGrid> where T : class
{
    public ScrollToSelectedItemBehavior(string str)
    {
        ChildrenProperty = typeof(T).GetProperty(str);
    }

    public PropertyInfo ChildrenProperty { get; }

    private readonly CompositeDisposable disposables = new();

    protected override void OnAttachedToVisualTree()
    {
        if (AssociatedObject is { SelectionInteraction: { } selection, RowSelection: { } rowSelection })
        {
            Observable.FromEventPattern(selection, nameof(selection.SelectionChanged))
                .Select(_ => rowSelection.SelectedItem as T)
                .WhereNotNull()
                .Throttle(TimeSpan.FromMilliseconds(100), Scheduler.CurrentThread)
                .Do(model => AssociatedObject.BringIntoView(model, GetChildren))
                .Subscribe()
                .DisposeWith(disposables);
        }
    }

    protected override void OnDetachedFromVisualTree()
    {
        disposables.Dispose();
    }

    private IEnumerable<T> GetChildren(T x)
    {
        return (IEnumerable<T>) ChildrenProperty.GetValue(x);
    }
}