using System.Reactive;

namespace Zafiro.Avalonia.Wizard;

internal class Navigator<T> : ReactiveObject where T : IValidatable
{
    private LinkedListNode<T>? current;

    public Navigator(IEnumerable<T> pagesList)
    {
        var linkedList = new LinkedList<T>(pagesList);
        Current = linkedList.First;
        var currentNodes = this.WhenAnyValue(x => x.Current);

        var hasNext = currentNodes.Select(node => node?.Next != null);
        var isValid = currentNodes.Select(x => x.Value).Select(validatable => validatable.IsValid).Switch();
        var validAndHasNext = hasNext.CombineLatest(isValid, (hasNext, isValid) => hasNext && isValid);

        GoNext = ReactiveCommand.Create(() => Current = Current!.Next, validAndHasNext);
        GoBack = ReactiveCommand.Create(() => Current = Current!.Previous, currentNodes.Select(x => x!.Previous != null));
        CurrentItems = currentNodes.Select(x => x!.Value);
        CurrentNodes = currentNodes.Select(node => node)!;
    }

    public IObservable<T> CurrentItems { get; set; }
    public ReactiveCommand<Unit, LinkedListNode<T>?> GoBack { get; }
    public ReactiveCommand<Unit, LinkedListNode<T>?> GoNext { get; }

    public LinkedListNode<T>? Current
    {
        get => current;
        set => this.RaiseAndSetIfChanged(ref current, value);
    }

    public IObservable<LinkedListNode<T>> CurrentNodes { get; }
}