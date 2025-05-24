using Microsoft.Extensions.DependencyInjection;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Services;
using Zafiro.UI.Shell;
using Zafiro.UI.Shell.Utils;

namespace TestApp.Shell;

public static class CompositionRoot
{
    public static MainViewModel Create()
    {
        ServiceCollection services = new();


        services.AddSingleton<IShell, Zafiro.UI.Shell.Shell>();
        services.AddSingleton(DialogService.Create());
        services.AddSingleton(NotificationService.Instance);
        services.RegisterAllSections(typeof(MainViewModel).Assembly);
        services.AddAllSections(typeof(MainViewModel).Assembly);
        services.AddTransient<MainViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<MainViewModel>();
    }
}