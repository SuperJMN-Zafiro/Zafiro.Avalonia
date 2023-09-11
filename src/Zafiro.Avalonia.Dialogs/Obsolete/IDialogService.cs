namespace Zafiro.Avalonia.Dialogs.Obsolete;

public interface IDialogService
{
    Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options);
}