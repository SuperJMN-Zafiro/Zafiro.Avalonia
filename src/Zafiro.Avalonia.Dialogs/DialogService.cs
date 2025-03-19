using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Zafiro.Avalonia.Dialogs;

public static class DialogService
{
    public static IDialog Create()
    {
        if (Application.Current is null)
        {
            throw new InvalidOperationException("Application is not initialized.");
        }
        
        return Application.Current.ApplicationLifetime switch
        {
            ISingleViewApplicationLifetime singleViewApplicationLifetime => DialogForSingleView(singleViewApplicationLifetime),
            _ => new StackedDialog()
        };
    }

    private static AdornerDialog DialogForSingleView(ISingleViewApplicationLifetime singleViewApplicationLifetime)
    {
        var mainView = singleViewApplicationLifetime.MainView;
        if (mainView == null)
        {
            throw new InvalidOperationException("Could not get the main view.");
        }

        return new AdornerDialog(mainView);
    }
}