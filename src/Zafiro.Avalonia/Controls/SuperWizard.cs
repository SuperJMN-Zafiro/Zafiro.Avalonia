using System.Reactive;
using System.Reactive.Linq;
using AvaloniaSyncer.Sections.NewSync;
using ReactiveUI;
using Zafiro.Avalonia.Wizard;
using Zafiro.Avalonia.Wizard.Interfaces;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.Controls;

public class SuperWizard<TPage1, TPage2, TResult> : ISuperWizard<TResult> where TPage1 : IValidatable where TPage2 : IValidatable
{
    public TPage1 Page1 { get; }
    public TPage2 Page2 { get; }

    private readonly TaskCompletionSource<TResult> tcs = new();

    public SuperWizard(Func<TPage1> page1factory, Func<TPage1, TPage2> page2factory)
    {
        PagesList = new List<IPage>
        {
            new Page<Unit, TPage1>(_ => page1factory(), "Next"),
            new Page<TPage1, TPage2>(page1 => page2factory(page1), "Next")
        };

        var navigator = new Navigator<IPage>(PagesList);
        GoNext = navigator.GoNext;
        GoBack = navigator.GoBack;
        CurrentPage = navigator.CurrentItems;
        

        navigator.CurrentNodes.Do(page =>
        {
            if (page.Value == PagesList.First())
            {
                page.Value.UpdateWith(Unit.Default);
            }
            else
            {
                page.Value.UpdateWith(page.Previous!.Value.Content);
            }
        }).Subscribe();

        var canComplete = navigator.CurrentItems.Select(x => PagesList.Last() == x);
        Complete = ReactiveCommand.Create(() => { }, canComplete);
        IsFinished = Complete.Any().ToSignal();
        IsFinished
            .Select(_ => (TResult)PagesList.Last().Content)
            .Do(result => tcs.SetResult(result))
            .Subscribe();
    }

    public ReactiveCommand<Unit, Unit> Complete { get; }
    public IObservable<IPage> CurrentPage { get; }
    public IList<IPage> PagesList { get; }
    public IReactiveCommand GoNext { get; }
    public IReactiveCommand GoBack { get; }
    public IObservable<Unit> IsFinished { get; }
    public Task<TResult> Result => tcs.Task;
}

