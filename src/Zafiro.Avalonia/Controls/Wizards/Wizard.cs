using System.Reactive;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Navigation;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.Wizards;

public partial class Wizard : ReactiveObject
{
    public INavigator Navigator { get; }
    public IEnhancedCommandOf<Unit, Unit> Next { get; }

    public Wizard(IList<Func<IValidatable>> pages, INavigator navigator)
    {
        Navigator = navigator;
        LinkedList<Func<IValidatable>> list = new(pages);

        var hasNext = this.WhenAnyValue<Wizard, LinkedListNode<Func<IValidatable>>?>(x => x.CurrentNode.Next).NotNull();
        var isValid = this.WhenAnyValue(x => x.Navigator.Content).WhereNotNull().Cast<IValidatable>().Select(x => x.IsValid).Switch();
        
        var canGoNext = hasNext.CombineLatest(isValid, (a, b) => a && b);

        var reactiveCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentNode == null)
            {
                CurrentNode = list.First;
            }
            else
            {
                CurrentNode = CurrentNode.Next;
            }
            
            navigator.Go(CurrentNode!.Value);
            
        }, canGoNext);

        Next = EnhancedCommand.Create(reactiveCommand);
        navigator.Back.Subscribe(_ => CurrentNode = CurrentNode!.Previous);

        reactiveCommand.Execute().Subscribe();
        IsLastPage = this.WhenAnyValue<Wizard, LinkedListNode<Func<IValidatable>>?>(x => x.CurrentNode).Select(x => x == list.Last);
        Back = navigator.Back;
    }

    public IEnhancedCommandOf<Unit, Unit> Back { get; }

    public IObservable<bool> IsLastPage { get; }

    [Reactive] private LinkedListNode<Func<IValidatable>>? currentNode;
}