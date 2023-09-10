namespace Zafiro.Avalonia.Dialogs;

public interface IDialogService
{
    Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options);
}