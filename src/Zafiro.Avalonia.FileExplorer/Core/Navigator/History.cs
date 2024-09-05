namespace Zafiro.Avalonia.FileExplorer.Core.Navigator;

public class History<T> : ReactiveObject, IHistory<T>
{
    private readonly Stack<T> currentFolderStack = new();

    public History()
    {
        var whenAnyValue = this.WhenAnyValue(x => x.CanGoBack);
        GoBack = ReactiveCommand.Create(OnBack, whenAnyValue);
    }

    private bool CanGoBack => currentFolderStack.Count > 1;

    public ReactiveCommand<Unit, Unit> GoBack { get; }

    public Maybe<T> CurrentFolder
    {
        get
        {
            if (currentFolderStack.Any())
            {
                return currentFolderStack.Peek();
            }

            return Maybe<T>.None;
        }
        set
        {
            if (currentFolderStack.Any() && Equals(value, currentFolderStack.Peek()))
            {
                return;
            }

            currentFolderStack.Push(value.GetValueOrThrow("The current folder should not be set to <none>"));
            this.RaisePropertyChanged(nameof(CanGoBack));
            this.RaisePropertyChanged(nameof(PreviousFolder));
            this.RaisePropertyChanged();
        }
    }

    public Maybe<T> PreviousFolder => currentFolderStack.SkipLast(1).TryFirst();

    private void OnBack()
    {
        currentFolderStack.Pop();
        this.RaisePropertyChanged(nameof(CurrentFolder));
        this.RaisePropertyChanged(nameof(PreviousFolder));
        this.RaisePropertyChanged(nameof(CanGoBack));
    }
}