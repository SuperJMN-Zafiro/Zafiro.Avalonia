using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Controls;
using JetBrains.Annotations;

namespace Zafiro.Avalonia;

[PublicAPI]
public static class ApplicationMixin
{
    public static void Connect(this Application application, Func<Control> createMainView, Func<Control, object> createDataContext)
    {
        var mainView = createMainView();

        switch (application.ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
            {
                var window = new Window
                {
                    Content = mainView,
                };

                desktop.MainWindow = window;
                break;
            }
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = mainView;
                break;
        }

        mainView.Loaded += (_, _) =>
        {
            mainView.DataContext = createDataContext(mainView);
        };
    }
}