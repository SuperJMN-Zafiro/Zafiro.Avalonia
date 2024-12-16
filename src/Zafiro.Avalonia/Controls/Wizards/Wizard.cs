using System.Reactive;
using System.Windows.Input;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Commands;
using Zafiro.Avalonia.Controls.Navigation;
using Zafiro.Reactive;

namespace Zafiro.Avalonia.Controls.Wizards;

public interface IWizard
{
    IEnhancedCommandOf<Unit, Unit> Back { get; }
    IEnhancedCommandOf<Unit, Unit> Next { get; }
    object Content { get; }
}

public partial class Wizard : ReactiveObject, IWizard
{
    [Reactive] private LinkedListNode<Func<IValidatable>>? currentNode;
    private object content;

    public IEnhancedCommandOf<Unit, Unit> Back { get; }
    public IEnhancedCommandOf<Unit, Unit> Next { get; }

    public object Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public Wizard(IList<Func<IValidatable>> pages)
    {
        LinkedList<Func<IValidatable>> list = new(pages);

        // Observa si hay siguiente página
        var hasNext = this.WhenAnyValue(x => x.CurrentNode)
                          .Select(node => node == null ? list.First : node.Next)
                          .Select(next => next != null);

        // Observa la validez del contenido actual
        var isValid = this.WhenAnyValue(x => x.Content)
                          .WhereNotNull()
                          .Cast<IValidatable>()
                          .Select(x => x.IsValid)
                          .Switch()
                          .StartWith(false);

        var canGoNext = hasNext.CombineLatest(isValid, (h, v) => h && v);

        var nextCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentNode == null)
            {
                CurrentNode = list.First;
            }
            else
            {
                CurrentNode = CurrentNode.Next;
            }

            var page = CurrentNode!.Value();
            Content = page;
        }, canGoNext);

        var backCommand = ReactiveCommand.Create(() =>
        {
            if (CurrentNode?.Previous != null)
            {
                CurrentNode = CurrentNode.Previous;
                var page = CurrentNode.Value();
                Content = page;
            }
        }, this.WhenAnyValue(x => x.CurrentNode)
          .Select(node => node?.Previous != null));

        Back = EnhancedCommand.Create(backCommand);
        Next = EnhancedCommand.Create(nextCommand);

        // Ejecuta Next para cargar la primera página
        nextCommand.Execute().Subscribe();
    }
}
