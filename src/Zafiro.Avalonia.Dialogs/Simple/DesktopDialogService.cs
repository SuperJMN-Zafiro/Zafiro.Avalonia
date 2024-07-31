using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs.Simple;

public class DesktopDialog : IDialog
{
    public DesktopDialog(Maybe<DataTemplates> dataTemplates)
    {
        DataTemplates = dataTemplates;
    }

    private static Window MainWindow =>
        ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;

    public Maybe<DataTemplates> DataTemplates { get; }

    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, Option[]> optionsFactory)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        var window = new Window
        {
            Title = title,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Icon = MainWindow.Icon
        };

        var closeable = new CloseableWrapper(window);
        var options = optionsFactory(closeable);
        var content = new DialogView
        {
            DataContext = new DialogViewModel(viewModel, options),
        };

        content.DataTemplates.AddRange(GetDialogTemplates());

        window.Content = new DialogViewContainer
        {
            Classes = { "Desktop" },
            Content = content,
        };

        SetWindowSize(window);

        if (Debugger.IsAttached)
        {
            window.AttachDevTools();
        }

        var result = await window.ShowDialog<bool?>(MainWindow).ConfigureAwait(false);
        return result is not (null or false);
    }

    private DataTemplates GetDialogTemplates()
    {
        var map = Application.Current.AsMaybe().Map(Dialog.GetTemplates);
        var templates = DataTemplates.Or(map);
        return templates.GetValueOrDefault(new DataTemplates());
    }

    private static void SetWindowSize(Window window)
    {
        window.SizeToContent = SizeToContent.WidthAndHeight;
    }
}