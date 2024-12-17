using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Dialogs.SizingAlgorithms;

namespace Zafiro.Avalonia.Dialogs;

public class DesktopDialog : IDialog
{
    public DesktopDialog(DataTemplates? dataTemplates = null, IChildSizingAlgorithm? algorithm = null)
    {
        DataTemplates = dataTemplates.AsMaybe();
        Algorithm = algorithm ?? OptimalSizeAlgorithm.Instance;
    }

    private static Window MainWindow =>
        ((IClassicDesktopStyleApplicationLifetime)Application.Current!.ApplicationLifetime!).MainWindow!;

    public Maybe<DataTemplates> DataTemplates { get; }
    public IChildSizingAlgorithm Algorithm { get; }

    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
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
            Content = viewModel,
            Options = options,
        };

        content.DataTemplates.AddRange(GetDialogTemplates());

        window.Content = new DialogViewContainer
        {
            Classes = { "Desktop" },
            Content = content,
        };

        SetWindowSize(window, content);

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

    private void SetWindowSize(Window window, Control content)
    {
        var screenFromWindow = MainWindow.Screens.ScreenFromWindow(window)!;
        var screenSize = new Size(screenFromWindow.Bounds.Size.Width, screenFromWindow.Bounds.Size.Height);
        var parentWindowSize = MainWindow.Bounds.Size;
        var size = Algorithm.GetWindowSize(content, screenSize, parentWindowSize);
        
        window.Width = size.Width;
        window.Height = size.Height;
    }
}