using System.Reactive;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia.Commands;

namespace Zafiro.Avalonia.Controls.Navigation;

public partial class Navigator : ReactiveObject, INavigator
{
    private readonly ObservableStack<Func<INavigator, Task<object>>> stack = new();
    
    public Navigator()
    {
        Back = EnhancedCommand.Create(ReactiveCommand.CreateFromTask(async () => await GoBack(), stack.Count.Select(b => b > 1)));
    }

    private async Task GoBack()
    {
        stack.Pop();
        var previous = stack.Top.Value;
        Content = await previous(this);
    }

    public IEnhancedCommandOf<Unit, Unit> Back { get; }

    public async Task Go(Func<INavigator, Task<object>> target)
    {
        stack.Push(target);
        Content = await target(this);
    }
    
    public async Task Go(Func<Task<object>> target)
    {
        stack.Push(async _ => await target());
        Content = await target();
    }

    // Métodos síncronos que llaman a los asíncronos
    public void Go(Func<INavigator, object> target)
    {
        _ = Go(nav => Task.FromResult(target(nav)));
    }
    
    public void Go(Func<object> target)
    {
        _ = Go(() => Task.FromResult(target()));
    }

    [Reactive]
    private object content;
}