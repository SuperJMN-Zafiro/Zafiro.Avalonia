using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Zafiro.Avalonia.Wizard.Interfaces;
using Zafiro.Mixins;

namespace Zafiro.Avalonia.Controls;

public class SuperWizard<TPage1, TPage2, TResult> : ISuperWizard<TResult> where TPage1 : IValidatable where TPage2 : IValidatable
{
    private readonly TaskCompletionSource<TResult> tcs = new();

    public SuperWizard(ISuperPage page1, ISuperPage page2, Func<TPage1, TPage2, TResult> selectResult)
    {
        PagesList = new List<ISuperPage>
        {
            page1,
            page2,
        };

        var navigator = new Navigator<ISuperPage>(PagesList);
        GoNext = navigator.GoNext;
        GoBack = navigator.GoBack;
        CurrentPage = navigator.CurrentItems;

        var canComplete = navigator.CurrentItems.Select(x => PagesList.Last() == x);
        Finish = ReactiveCommand.Create(() => { }, canComplete);
        IsFinished = Finish.Any().ToSignal();
        IsFinished
            .Select(_ => selectResult((TPage1) page1.Content, (TPage2) page2.Content))
            .Do(result => tcs.SetResult(result))
            .Subscribe();
    }

    public ReactiveCommand<Unit, Unit> Finish { get; }
    public IObservable<ISuperPage> CurrentPage { get; }
    public IList<ISuperPage> PagesList { get; }
    public IReactiveCommand GoNext { get; }
    public IReactiveCommand GoBack { get; }
    public IObservable<Unit> IsFinished { get; }
    public string FinishText => PagesList.Last().NextText;
    public Task<TResult> Result => tcs.Task;
}

public class SuperPage<T> : ISuperPage
{
    public SuperPage(IValidatable content, string next)
    {
        Content = content;
        NextText = next;
    }

    public string NextText { get; set; }

    public object Content { get; set; }
}

public interface ISuperPage
{
    object Content { get; }
    string NextText { get;  }
}

