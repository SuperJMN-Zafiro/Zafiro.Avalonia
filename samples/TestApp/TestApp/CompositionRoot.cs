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
        services.AddSingleton(new ShellProperties("Avalonia.Zafiro Tookit", sectionContent => CreateSectionContentHeader(sectionContent)));
        services.AddSingleton(DialogService.Create());
        services.AddSingleton(NotificationService.Instance);
        services.RegisterAllSections(typeof(MainViewModel).Assembly);
        services.AddAllSections(typeof(MainViewModel).Assembly);
        services.AddTransient<MainViewModel>();
        services.AddTransient<TargetViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<MainViewModel>();
    }

    private static IObservable<object?> CreateSectionContentHeader(object sectionContent)
    {
        var content = (SectionScope)sectionContent;
        return content.Navigator.Content.Select(o => o.GetType().GetCustomAttribute<SectionAttribute>().Name ?? GetSectionName(o.GetType()));
    }

    private static string GetSectionName(Type getType)
    {
        string sectionName = getType.Name.Replace("ViewModel", "");
        string formattedName = string.Concat(sectionName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        return formattedName;
    }
}