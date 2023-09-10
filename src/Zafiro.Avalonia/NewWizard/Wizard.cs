using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using ReactiveUI;
using Zafiro.Avalonia.NewWizard.Interfaces;

namespace Zafiro.Avalonia.NewWizard;

public class Wizard<T> : ReactiveObject, IWizard<T, IPage<IValidatable, IValidatable>>, IWizard
{
    private LinkedListNode<IPage<IValidatable, IValidatable>>? current;
    private readonly TaskCompletionSource<T> tcs = new();
    private readonly ReactiveCommand<Unit, Unit> goNextReactive;
    private readonly ReactiveCommand<Unit, Unit> goBackReactive;

    public Wizard(IList<IPage<IValidatable, IValidatable>> pages)
    {
        Pages = new LinkedList<IPage<IValidatable, IValidatable>>(pages);
        Current = Pages.First;
        
        var isValid = this.WhenAnyObservable(x => x.Current.Value.Content.IsValid);

        goNextReactive = ReactiveCommand.Create(() =>
        {
            if (Current == Pages.Last)
            {
                tcs.SetResult((T)Current.Value.Content);
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

        CurrentPageWizard = this.WhenAnyValue(x => x.Current).Select(x => x.Value);

        GoBack = new Command<Unit, Unit>(goBackReactive);
    }

    public IObservable<object> CurrentPageWizard { get; }
    public IList<object> PagesList => Pages.Cast<object>().ToList();

    public ICommand GoNextCommand => goNextReactive;

    public ICommand GoBackCommand => goBackReactive;
    public IObservable<bool> CanGoNext => goNextReactive.CanExecute;
    public IObservable<bool> CanGoBack => goBackReactive.CanExecute;

    Task<object> IWizard.Result => tcs.Task as Task<object>;

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