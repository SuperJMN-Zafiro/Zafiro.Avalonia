using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.Adorners;
using Zafiro.Avalonia;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Services;
using Zafiro.UI.Navigation;
using Zafiro.UI.Shell;

namespace TestApp.Shell;

public static class CompositionRoot
{
    public static MainViewModel Create()
    {
        ServiceCollection services = new();


        services.AddSingleton<IShell, Zafiro.UI.Shell.Shell>();
        services.AddSingleton(DialogService.Create());
        services.AddSingleton(NotificationService.Instance);
        services.RegisterAllViewModels(typeof(MainViewModel).Assembly);
        services.RegisterAllSections(typeof(MainViewModel).Assembly);
        services.AddTransient<MainViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<MainViewModel>();
    }
}

public static class ServiceCollectionExtensions
{
    public static IServiceCollection RegisterAllViewModels(this IServiceCollection services, Assembly assembly)
    {
        // Encuentra todos los tipos que terminan con "ViewModel"
        var viewModelTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("ViewModel"));

        foreach (var viewModelType in viewModelTypes)
        {
            // Registra cada ViewModel como scoped
            services.AddScoped(viewModelType);
        }

        return services;
    }
}

public static class NavigationExtensions
{
    public static IServiceCollection RegisterAllSections(this IServiceCollection services, Assembly assembly)
    {
        services.RegisterSections(builder =>
        {
            var viewModelTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.Name.EndsWith("ViewModel") && t.FullName.Contains("Samples"));

            foreach (var viewModelType in viewModelTypes)
            {
                // Elimina el sufijo "ViewModel" para el nombre de la sección
                string sectionName = viewModelType.Name.Replace("ViewModel", "");

                // Crea el nombre formateado con espacios entre palabras (CamelCase -> "Camel Case")
                string formattedName = string.Concat(sectionName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');

                // Agrega la sección con un icono predeterminado
                var method = typeof(SectionsBuilder).GetMethod("Add")?.MakeGenericMethod(viewModelType);
                var iconId = viewModelType.GetCustomAttribute<IconAttribute>()?.Id ?? "fa-comment-dots"; // Icono predeterminado
                method?.Invoke(builder, new object[] { formattedName, new IconViewModel { IconId = iconId }, true });
            }
        });

        return services;
    }
}