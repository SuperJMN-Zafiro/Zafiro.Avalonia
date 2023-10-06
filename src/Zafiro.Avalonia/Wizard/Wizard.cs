using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.WizardOld.Interfaces;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.Wizard;

public class Wizard<TPage1, TPage2, TResult> : IWizard<TResult> where TPage1 : IValidatable where TPage2 : IValidatable
{
    private readonly TaskCompletionSource<TResult> tcs = new();

    public Wizard(IPage page1, IPage page2, Func<TPage1, TPage2, TResult> selectResult)
    {
        PagesList = new List<IPage>
        {
            page1,
            page2,
        };

        var navigator = new Navigator<IPage>(PagesList);
        GoNext = navigator.GoNext;
        GoBack = navigator.GoBack;
        CurrentPage = navigator.CurrentItems;

        var canComplete = navigator.CurrentItems.Select(x => PagesList.Last() == x);
        var lastIsValid = page2.Content.IsValid;
        Finish = ReactiveCommand.Create(() => { }, canComplete.CombineLatest(lastIsValid, (canComplete, lastIsValid) => canComplete && lastIsValid));
        IsFinished = Finish.Any().ToSignal();
        IsFinished
            .Select(_ => selectResult((TPage1) page1.Content, (TPage2) page2.Content))
            .Do(result => tcs.SetResult(result))
            .Subscribe();
    }

    public ReactiveCommand<Unit, Unit> Finish { get; }
    public IObservable<IPage> CurrentPage { get; }
    public IList<IPage> PagesList { get; }
    public IReactiveCommand GoNext { get; }
    public IReactiveCommand GoBack { get; }
    public IObservable<Unit> IsFinished { get; }
    public string FinishText => PagesList.Last().NextText;
    public Task<TResult> Result => tcs.Task;
}