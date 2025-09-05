using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.Navigation;
using TestApp.Shell;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Services;
using Zafiro.UI.Navigation;
using Zafiro.UI.Shell;
using Zafiro.UI.Shell.Utils;

namespace TestApp;

public static class CompositionRoot
{
    public static MainViewModel Create()
    {
        ServiceCollection services = new();

        services.AddSingleton<IShell, Zafiro.UI.Shell.Shell>();
        services.AddSingleton(new ShellProperties("Avalonia.Zafiro Tookit", navigatorObj => CreateHeaderFromNavigator(navigatorObj)));
        services.AddSingleton(DialogService.Create());
        // Defer NotificationService initialization on Android to avoid early TopLevel access
        // services.AddSingleton(NotificationService.Instance);
        services.RegisterAllSections(typeof(MainViewModel).Assembly);
        services.AddAllSections(typeof(MainViewModel).Assembly);
        services.AddTransient<MainViewModel>();
        services.AddTransient<TargetViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        // Diagnostics: log Shell sections to verify linking/reflection behavior on Android
        try
        {
            var shell = serviceProvider.GetRequiredService<IShell>();
            var sectionsCount = shell.Sections?.Count() ?? 0;
            Console.WriteLine($"[TestApp] Shell Sections: {sectionsCount}");

            // Force a known initial section to ensure visible content
            const string initialSection = "Slim Data Grid";
            try
            {
                shell.GoToSection(initialSection);
                Console.WriteLine($"[TestApp] Forced initial section: {initialSection}");
            }
            catch (Exception inner)
            {
                Console.WriteLine($"[TestApp] Error forcing initial section: {inner}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TestApp] Error resolving IShell: {ex}");
        }

        return serviceProvider.GetRequiredService<MainViewModel>();
    }

    private static IObservable<object?> CreateHeaderFromNavigator(object navigatorObj)
    {
        var navigator = (INavigator)navigatorObj;
        return navigator.Content.Select(o =>
        {
            var type = o?.GetType();

            var s = type?.GetCustomAttribute<SectionAttribute>()?.Name;
            if (s != null)
            {
                return s;
            }

            if (type is null)
            {
                return "Unknown Section";
            }

            return GetSectionName(type);
        });
    }

    private static string GetSectionName(Type getType)
    {
        string sectionName = getType.Name.Replace("ViewModel", "");
        string formattedName = string.Concat(sectionName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        return formattedName;
    }
}