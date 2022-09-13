using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;

namespace MultiSelectorSample.Views;

public class MultiSelector : ItemsControl, IDisposable
{
    private readonly CompositeDisposable disposables = new();

    public MultiSelector()
    {
        var changes = this
            .WhenAnyValue(x => x.Items)
            .WhereNotNull()
            .Select(x => x.Cast<ISelectable>())
            .Select(x => x.AsObservableChangeSet())
            .Switch();
        
        var isSelected = changes
            .AutoRefresh(x => x.IsSelected)
            .ToCollection()
            .Select(GetSelectionState);

        var isSelectedSubject = new BehaviorSubject<bool?>(false);
        isSelected.Subscribe(isSelectedSubject);
        
        Toggle = ReactiveCommand.CreateFromObservable(() =>
            {
                bool? currentValue = isSelectedSubject.Value;
                return ToggleChildrenSelection(changes, GetNextValue(currentValue));
            })
            .DisposeWith(disposables);

        IsChecked = isSelected
            .CombineLatest(Toggle.IsExecuting)
            .Where(x => !x.Second)
            .Select(x => x.First)
            .Replay(1)
            .RefCount();
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

    private static IObservable<IReadOnlyCollection<ISelectable>> ToggleChildrenSelection(
        IObservable<IChangeSet<ISelectable>> changes, bool getNextValue)
    {
        return changes
            .ToCollection()
            .Do(x =>
            {
                var isChildSelected = getNextValue;
                x.ToList().ForEach(notify => notify.IsSelected = isChildSelected);
            })
            .Take(1);
    }

    private static bool GetNextValue(bool? currentValue)
    {
        return !currentValue ?? false;
    }
}