using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Dialogs.Simple;

public class DesktopDialog : IDialog
{
    public DesktopDialog(Application application) : this(Maybe.From(Dialog.GetTemplates(application)))
    {
        DataTemplates = Maybe.From(Dialog.GetTemplates(application));
    }

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
            DataContext = new DialogViewModel(viewModel, options)
        };

        DataTemplates.Execute(content.DataTemplates.AddRange);

        window.Content = new DialogViewContainer
        {
            Classes = { "Desktop" },
            Content = content
        };

        SetWindowSize(window);

#if DEBUG
        window.AttachDevTools();
#endif
        var result = await window.ShowDialog<bool?>(MainWindow).ConfigureAwait(false);
        return result is not (null or false);
    }

    private static void SetWindowSize(Window window)
    {
        window.SizeToContent = SizeToContent.WidthAndHeight;

        window.Height = MainWindow.Bounds.Height / 3;
        window.Width = MainWindow.Bounds.Width / 3;
    }
}