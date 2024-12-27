using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls.Templates;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Dialogs;

public class DesktopDialog : IDialog
{
    public DesktopDialog(DataTemplates? dataTemplates = null)
    {
        DataTemplates = dataTemplates.AsMaybe();
    }

    private static Result<Window> MainWindow => Application.Current!.TopLevel().Map(level => level as Window).EnsureNotNull("TopLevel is not a Window!");

    public Maybe<DataTemplates> DataTemplates { get; }

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
        
        if (Debugger.IsAttached)
        {
            window.AttachDevTools();
        }

        var result = await window.ShowDialog<bool?>(MainWindow.Value).ConfigureAwait(false);
        return result is not (null or false);
    }

    private DataTemplates GetDialogTemplates()
    {
        var map = Application.Current.AsMaybe().Map(Dialog.GetTemplates);
        var templates = DataTemplates.Or(map);
        return templates.GetValueOrDefault(new DataTemplates());
    }
}