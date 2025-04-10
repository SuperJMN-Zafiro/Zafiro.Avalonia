using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input.Platform;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;

namespace Zafiro.Avalonia.Mixins;

[PublicAPI]
public static class ApplicationMixin
{
    public static Result<IClipboard> GetClipboard()
    {
        return TopLevel(Application.Current).Bind(x => x.Clipboard.AsMaybe().ToResult("Clipboard is null"));
    }
    
    public static Result<TopLevel> TopLevel(this Application? app)
    {
        if (app == null)
        {
            return Result.Failure<TopLevel>("Application is null");
        }
        
        if (app.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime { MainWindow: { } window })
        {
            return window;
        }

        if (app?.ApplicationLifetime is ISingleViewApplicationLifetime { MainView: { } mainView })
        {
            var topLevel = global::Avalonia.Controls.TopLevel.GetTopLevel(mainView);
            if (topLevel is not null)
            {
                return topLevel;
            }

            return Result.Failure<TopLevel>("Could not find the top level");
        }

        return Result.Failure<TopLevel>("No top-level application available");
    }
    
    public static void Connect(this Application application, Func<Control> createMainView, 
        Func<Control, object> createDataContext, Func<Window>? createApplicationWindow = default)
    {
        ConnectImpl(application, createMainView, control => Task.FromResult(createDataContext(control)), createApplicationWindow);
    }

    public static void Connect(this Application application, Func<Control> createMainView, 
        Func<Control, Task<object>> createDataContext, Func<Window>? createApplicationWindow = default)
        => ConnectImpl(application, createMainView, createDataContext, createApplicationWindow);

    private static void ConnectImpl(Application application, Func<Control> createMainView, 
        Func<Control, Task<object>> createDataContext, Func<Window>? createApplicationWindow)
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
        mainView.Loaded += async (_, _) =>
        {
            var dataContext = await createDataContext(mainView);
            mainView.DataContext = dataContext;
        };
    }
}