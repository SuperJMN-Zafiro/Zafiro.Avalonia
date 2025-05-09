using System.Reactive;
using Zafiro.Avalonia.Controls.Wizards.Builder;
using Zafiro.Reactive;
using Zafiro.UI.Commands;

namespace Zafiro.Avalonia.Controls.Wizards;

public class Wizard<TResult> : ReactiveObject, IWizard<TResult>
{
    private readonly IList<IStep?> createdPages;
    private readonly IList<Func<IStep?, IStep>> pageFactories;
    private readonly Func<IStep, TResult> resultFactory;
    private IStep content;
    private int currentIndex = -1;

    public Wizard(List<Func<IStep?, IStep>> pages, Func<IStep, TResult> resultFactory)
    {
        this.resultFactory = resultFactory;
        pageFactories = pages;
        createdPages = pages.Select(_ => (IStep?)null).ToList();

        var hasNext = CreateHasNextObservable();
        IsValid = CreateIsValidObservable();
        IsBusy = CreateIsBusyObservable();
        IsLastPage = hasNext.Not();

        var nextCommand = CreateNextCommand(hasNext, IsValid);
        var backCommand = CreateBackCommand();

        // Se elimina la suscripción reactiva a la generación del resultado.
        SetupCommands(nextCommand, backCommand);
        InitializeWizard(nextCommand);
    }

    public TResult Result { get; private set; }

    public int CurrentIndex
    {
        get => currentIndex;
        set => this.RaiseAndSetIfChanged(ref currentIndex, value);
    }

    public TResult GetResult()
    {
        return resultFactory(Content);
    }

    public IEnhancedCommand Back { get; private set; }
    public IEnhancedCommand Next { get; private set; }

    public IStep Content
    {
        get => content;
        set => this.RaiseAndSetIfChanged(ref content, value);
    }

    public IObservable<bool> IsLastPage { get; }
    public IObservable<bool> IsValid { get; }
    public IObservable<bool> IsBusy { get; }
    public IObservable<int> PageIndex => this.WhenAnyValue(x => x.CurrentIndex);
    public int TotalPages => pageFactories.Count;

    private IObservable<bool> CreateHasNextObservable() =>
        this.WhenAnyValue(x => x.CurrentIndex)
            .Select(i => i < pageFactories.Count - 1);

    private IObservable<bool> CreateIsValidObservable() =>
        this.WhenAnyValue(x => x.Content)
            .WhereNotNull()
            .Select(x => x.IsValid)
            .Switch()
            .StartWith(false);

    private IObservable<bool> CreateIsBusyObservable() =>
        this.WhenAnyValue(x => x.Content)
            .WhereNotNull()
            .Select(x => x.IsBusy)
            .Switch()
            .StartWith(false);

    private ReactiveCommand<Unit, Unit> CreateNextCommand(IObservable<bool> hasNext, IObservable<bool> isValid)
    {
        var canGoNext = hasNext.CombineLatest(isValid, (h, v) => h && v);
        return ReactiveCommand.Create(NavigateNext, canGoNext);
    }

    private void NavigateNext()
    {
        CurrentIndex++;
        EnsurePageCreated();
        Content = createdPages[CurrentIndex]!;
    }

    private void EnsurePageCreated()
    {
        if (createdPages[CurrentIndex] != null) return;

        var previousPage = CurrentIndex > 0 ? createdPages[CurrentIndex - 1] : null;
        createdPages[CurrentIndex] = pageFactories[CurrentIndex](previousPage);
    }

    private ReactiveCommand<Unit, Unit> CreateBackCommand()
    {
        var isFirstPage = this.WhenAnyValue(x => x.CurrentIndex).Select(i => i == 0);
        var canGoBack = isFirstPage.CombineLatest(IsBusy, IsLastPage,
            (first, busy, last) => !first && !busy && !last);

        return ReactiveCommand.Create(NavigateBack, canGoBack);
    }

    private void NavigateBack()
    {
        createdPages[CurrentIndex] = null;
        CurrentIndex--;
        Content = createdPages[CurrentIndex]!;
    }

    private void SetupCommands(ReactiveCommand<Unit, Unit> next, ReactiveCommand<Unit, Unit> back)
    {
        Next = next.Enhance();
        Back = back.Enhance();
    }

    private void InitializeWizard(ReactiveCommand<Unit, Unit> nextCommand)
    {
        SetupAutoAdvance(nextCommand);
        nextCommand.Execute().Subscribe();
    }

    private IDisposable SetupAutoAdvance(ReactiveCommand<Unit, Unit> nextCommand) =>
        IsValid.Trues()
            .Where(_ => Content.AutoAdvance)
            .ToSignal()
            .InvokeCommand(nextCommand);
}