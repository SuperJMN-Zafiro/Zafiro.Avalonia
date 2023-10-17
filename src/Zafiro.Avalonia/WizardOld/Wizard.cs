using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Zafiro.Avalonia.Model;
using Zafiro.Avalonia.WizardOld.Interfaces;

namespace Zafiro.Avalonia.WizardOld;

public class Wizard<T> : ReactiveObject, IWizard<T, IPage<IValidatable, IValidatable>>, IWizard
{
    private LinkedListNode<IPage<IValidatable, IValidatable>>? current;
    private readonly TaskCompletionSource<T> tcs = new();
    private readonly ReactiveCommand<Unit, Unit> goNext;
    private readonly ReactiveCommand<Unit, Unit> goBack;
    private readonly BehaviorSubject<bool> isFinished = new(false);

    public Wizard(IList<IPage<IValidatable, IValidatable>> pages)
    {
        if (!pages.Any())
        {
            throw new ArgumentException("There must be at least one page in the Wizard", nameof(pages));
        }

        Pages = new LinkedList<IPage<IValidatable, IValidatable>>(pages);
        Current = Pages.First!;

        var isValid = this.WhenAnyObservable(x => x.Current.Value.Content.IsValid);
        var canGoBack = this.WhenAnyValue(x => x.Current).Select(node => node != Pages.First);

        goNext = ReactiveCommand.Create(() =>
        {
            if (isFinished.Value)
            {
                return;
            }

            if (Current == Pages.Last)
            {
                tcs.SetResult((T) Current.Value.Content);
                isFinished.OnNext(true);
            }
            else
            {
                Current = Current.Next!;
            }
        }, isValid.CombineLatest(isFinished, (isValid, isFinished) => isValid && !isFinished));
        GoNext = new Command<Unit, Unit>(goNext);

        goBack = ReactiveCommand.Create(() => { Current = Current.Previous!; }, canGoBack);

        CurrentPageWizard = this.WhenAnyValue(x => x.Current).Select(x => x.Value).Cast<IPage>();

        GoBack = new Command<Unit, Unit>(goBack);
    }

    public IObservable<bool> IsFinished => isFinished.AsObservable();

    public IObservable<IPage> CurrentPageWizard { get; }
    public IList<IPage> PagesList => Pages.Cast<IPage>().ToList();

    public IReactiveCommand GoNextCommand => goNext;

    public IReactiveCommand GoBackCommand => goBack;

    public IMyCommand GoNext { get; }
    public IMyCommand GoBack { get; }
    public Task<T> Result => tcs.Task;

    public void SetResult(T result)
    {
        tcs.SetResult(result);
    }

    public IObservable<IPage<IValidatable, IValidatable>> CurrentPage => this.WhenAnyValue(x => x.Current).Select(x => x.Value);

    private LinkedList<IPage<IValidatable, IValidatable>> Pages { get; }

    private LinkedListNode<IPage<IValidatable, IValidatable>>? Current
    {
        get => current;
        set
        {
            IValidatable content = null;
            if (current != null)
            {
                content = current.Value.Content;
            }

            current = value;
            current.Value.UpdateWith(content);
            this.RaisePropertyChanged();
        }
    }
}