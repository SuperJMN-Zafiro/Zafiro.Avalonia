using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia.Controls;
using DynamicData;
using ReactiveUI;

namespace MultiSelectorSample.Views;

public class MultiSelector : ItemsControl
{
    private bool canUpdate = true;

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

        IsChecked = isSelected
            .Where(_ => canUpdate)
            .Replay(1)
            .RefCount();

        Toggle = ReactiveCommand.CreateFromObservable(() => ToggleChildrenSelection(changes, isSelectedSubject));
    }

    private IObservable<IReadOnlyCollection<ISelectable>> ToggleChildrenSelection(IObservable<IChangeSet<ISelectable>> changes, BehaviorSubject<bool?> subject)
    {
        return changes
            .ToCollection()
            .Do(x =>
            {
                var toSet = !subject.Value ?? false;
                canUpdate = false;
                x.ToList().ForEach(notify => notify.IsSelected = toSet);
                canUpdate = true;
            })
            .Take(1);
    }

    public ReactiveCommand<Unit, IReadOnlyCollection<ISelectable>> Toggle { get; }

    private static bool? GetSelectionState(IReadOnlyCollection<ISelectable> collection)
    {
        return collection.All(x => x.IsSelected) ? true : collection.Any(x => x.IsSelected) ? (bool?) null : false;
    }

    public IObservable<bool?> IsChecked { get; }
}