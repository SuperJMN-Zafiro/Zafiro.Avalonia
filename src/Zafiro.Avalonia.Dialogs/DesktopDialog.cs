using Avalonia;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Dialogs.Views;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Dialogs;

public class DesktopDialog : IDialog
{
    private static Result<Window> MainWindow => Application.Current!.TopLevel().Map(level => level as Window).EnsureNotNull("TopLevel is not a Window!");

    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var window = new Window
        {
            Title = title,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Icon = MainWindow.Value.Icon,
            SizeToContent = SizeToContent.WidthAndHeight,
            MaxWidth = 800,
            MaxHeight = 800,
            MinWidth = 300,
            MinHeight = 200
        };

        var closeable = new CloseableWrapper(window);
        var options = optionsFactory(closeable);

        window.Content = new DialogControl
        {
            Title = Maybe<string>.None,
            Content = viewModel,
            Options = options
        };

        var result = await window.ShowDialog<bool?>(MainWindow.Value).ConfigureAwait(false);
        return result is not (null or false);
    }
}