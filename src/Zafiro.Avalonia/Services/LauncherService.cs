using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;

namespace Zafiro.Avalonia.Services;

public class LauncherService(ILauncher launcher) : ILauncherService
{
    public Task LaunchUri(Uri uri) => launcher.LaunchUriAsync(uri);
    
    public static LauncherService Instance => new(TopLevel().Launcher);

    private static TopLevel TopLevel()
    {
        if (Application.Current?.ApplicationLifetime is null)
        {
            throw new InvalidOperationException("This application is not supported");
        }

        return Application.Current.ApplicationLifetime switch
        {
            IClassicDesktopStyleApplicationLifetime classicDesktopStyleApplicationLifetime => classicDesktopStyleApplicationLifetime.MainWindow!,
            ISingleViewApplicationLifetime singleViewApplicationLifetime => global::Avalonia.Controls.TopLevel.GetTopLevel(singleViewApplicationLifetime.MainView)!,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}