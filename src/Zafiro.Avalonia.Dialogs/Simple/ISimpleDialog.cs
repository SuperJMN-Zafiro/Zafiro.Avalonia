namespace Zafiro.Avalonia.Dialogs.Simple;

public interface ISimpleDialog
{
    public Task Show(object viewModel, string title, IObservable<bool> canSubmit);
}