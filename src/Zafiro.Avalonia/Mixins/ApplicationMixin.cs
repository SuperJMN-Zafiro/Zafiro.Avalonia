using Avalonia.Controls.ApplicationLifetimes;
using JetBrains.Annotations;

namespace Zafiro.Avalonia.Mixins;

[PublicAPI]
public static class ApplicationMixin
{
    public static void Connect(this Application application, Func<Control> createMainView, Func<Control, object> createDataContext, Func<Window>? createApplicationWindow = default)
    {
        var mainView = createMainView();

        switch (application.ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
            {
                var window = createApplicationWindow?.Invoke() ?? new Window();

                window.Content = mainView;

                desktop.MainWindow = window;
                break;
            }
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = mainView;
                break;
        }

        mainView.Loaded += (_, _) =>
        {
            var dataContext = createDataContext(mainView);
            mainView.DataContext = dataContext;
        };
    }
}