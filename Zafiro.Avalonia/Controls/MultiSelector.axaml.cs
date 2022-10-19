using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls;

public class MultiSelector : ItemsControl, IDisposable
{
    private readonly CompositeDisposable disposables = new();

    public MultiSelector()
    {
        var changes = this
            .WhenAnyValue(x => x.Items)
            .Where(x => x is IEnumerable<ISelectable>)
            .Select(collection => new Container<ISelectable>(collection))
            .Select(x => x.ToObservableChangeSet<Container<ISelectable>, ISelectable>())
            .Switch();

        var isSelected = changes
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Select(GetSelectionState);

        var isSelectedSubject = new BehaviorSubject<bool?>(false);
        isSelected.Subscribe(isSelectedSubject);

        Toggle = ReactiveCommand.CreateFromObservable(() =>
            {
                var currentValue = isSelectedSubject.Value;
                return ToggleChildrenSelection(changes, GetNextValue(currentValue));
            })
            .DisposeWith(disposables);

        IsChecked = isSelected
            .CombineLatest(Toggle.IsExecuting)
            .Where(x => !x.Second)
            .Select(x => x.First)
            .ReplayLastActive();
    }

    public ReactiveCommand<Unit, IReadOnlyCollection<ISelectable>> Toggle { get; }

    public IObservable<bool?> IsChecked { get; }

    public void Dispose()
    {
        disposables.Dispose();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        disposables.Dispose();
        base.OnDetachedFromVisualTree(e);
    }

    private static bool? GetSelectionState(IReadOnlyCollection<ISelectable> collection)
    {
        return collection.All(x => x.IsSelected) ? true : collection.Any(x => x.IsSelected) ? null : false;
    }

    private static IObservable<IReadOnlyCollection<ISelectable>> ToggleChildrenSelection(IObservable<IChangeSet<ISelectable>> changes, bool getNextValue)
    {
        return changes
            .ToCollection()
            .Do(
                collection =>
                {
                    var isChildSelected = getNextValue;
                    collection.ToList().ForEach(notify => notify.IsSelected = isChildSelected);
                })
            .Take(1);
    }

    private static bool GetNextValue(bool? currentValue)
    {
        return !currentValue ?? false;
    }

    private class Container<T> : INotifyCollectionChanged, IEnumerable<T>
    {
        private readonly IEnumerable<T> enumerable;
        private readonly INotifyCollectionChanged notifyCollectionChanged;

        public Container(object source)
        {
            enumerable = (IEnumerable<T>) source;
            notifyCollectionChanged = source as INotifyCollectionChanged ?? new ObservableCollection<T>();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) enumerable).GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler? CollectionChanged
        {
            add => notifyCollectionChanged.CollectionChanged += value;
            remove => notifyCollectionChanged.CollectionChanged -= value;
        }
    }
}