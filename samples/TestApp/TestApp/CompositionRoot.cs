using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.Navigation;
using TestApp.Shell;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Services;
using Zafiro.UI.Shell;
using Zafiro.UI.Shell.Utils;

namespace TestApp;

public static class CompositionRoot
{
    public static MainViewModel Create()
    {
        ServiceCollection services = new();

        services.AddSingleton<IShell, Zafiro.UI.Shell.Shell>();
        services.AddSingleton(new ShellProperties("Avalonia.Zafiro Tookit"));
        services.AddSingleton(DialogService.Create());
        services.AddSingleton(NotificationService.Instance);
        services.RegisterAllSections(typeof(MainViewModel).Assembly);
        services.AddAllSections(typeof(MainViewModel).Assembly);
        services.AddTransient<MainViewModel>();
        services.AddTransient<TargetViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<MainViewModel>();
    }
}