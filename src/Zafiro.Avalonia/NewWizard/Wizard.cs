using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;
using Zafiro.Avalonia.NewWizard.Interfaces;

namespace Zafiro.Avalonia.NewWizard;

public class Wizard<T> : ReactiveObject, IWizard<T, IPage<IValidatable, IValidatable>>, IWizard
{
    private LinkedListNode<IPage<IValidatable, IValidatable>>? current;
    private readonly TaskCompletionSource<T> tcs = new();
    private readonly ReactiveCommand<Unit, Unit> goNextReactive;
    private readonly ReactiveCommand<Unit, Unit> goBackReactive;
    private readonly BehaviorSubject<bool> isFinished = new(false);

    public Wizard(IList<IPage<IValidatable, IValidatable>> pages)
    {
        Pages = new LinkedList<IPage<IValidatable, IValidatable>>(pages);
        Current = Pages.First;
        
        var isValid = this.WhenAnyObservable(x => x.Current.Value.Content.IsValid);

        goNextReactive = ReactiveCommand.Create(() =>
        {
            if (isFinished.Value)
            {
                return;
            }

            if (Current == Pages.Last)
            {
                tcs.SetResult((T)Current.Value.Content);
                isFinished.OnNext(true);
            }
            else
            {
                Current = Current.Next;
            }
        }, isValid);
        GoNext = new Command<Unit, Unit>(goNextReactive);

        goBackReactive = ReactiveCommand.Create(() =>
        {
            Current = Current.Previous;
        });

        CurrentPageWizard = this.WhenAnyValue(x => x.Current).Select(x => x.Value).Cast<IPage>();

        GoBack = new Command<Unit, Unit>(goBackReactive);
    }

    public IObservable<bool> IsFinished => isFinished.AsObservable();

    public IObservable<IPage> CurrentPageWizard { get; }
    public IList<IPage> PagesList => Pages.Cast<IPage>().ToList();

    public IReactiveCommand GoNextCommand => goNextReactive;

    public IReactiveCommand GoBackCommand => goBackReactive;
    public IObservable<bool> CanGoNext => goNextReactive.CanExecute;
    public IObservable<bool> CanGoBack => goBackReactive.CanExecute;

    public IMyCommand GoNext { get; }
    public IMyCommand GoBack { get; }
    public Task<T> Result => tcs.Task;

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