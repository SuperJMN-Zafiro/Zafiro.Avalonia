using Zafiro.Avalonia.Dialogs.Obsolete;

namespace Zafiro.Avalonia.Dialogs.Simple;

public interface ISimpleDialog
{
    Task Show(object viewModel, string title, Func<ICloseable, Option[]> optionsFactory);
}