using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Zafiro.Avalonia.Controls;

internal class Navigator<T> : ReactiveObject
{
    public Navigator(IEnumerable<T> pagesList)
    {
        var linkedList = new LinkedList<T>(pagesList);
        Current = linkedList.First;
        var currentNodes = this.WhenAnyValue(x => x.Current);
        GoNext = ReactiveCommand.Create(() => Current = Current!.Next, currentNodes.Select(x => x!.Next != null));
        GoBack = ReactiveCommand.Create(() => Current = Current!.Previous, currentNodes.Select(x => x!.Previous != null));
        CurrentItems = currentNodes.Select(x => x!.Value);
        CurrentNodes = currentNodes.Select(node => node)!;
    }

    public IObservable<T> CurrentItems { get; set; }

    public ReactiveCommand<Unit, LinkedListNode<T>?> GoBack { get; }

    public ReactiveCommand<Unit, LinkedListNode<T>?> GoNext { get; }

    [Reactive] private LinkedListNode<T>? Current { get; set; }
    public IObservable<LinkedListNode<T>> CurrentNodes { get; }
}