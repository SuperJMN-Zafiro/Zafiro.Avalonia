using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.Adorners;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.SlimWizard;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.DataAnalysis.Dendrograms;
using TestApp.Samples.DataAnalysis.Heatmaps;
using TestApp.Samples.DataAnalysis.Monitoring;
using TestApp.Samples.DataAnalysis.Tables;
using TestApp.Samples.Diagrams;
using TestApp.Samples.Dialogs;
using TestApp.Samples.Drag;
using TestApp.Samples.Misc;
using TestApp.Samples.NumberOnlyTextBox;
using TestApp.Samples.Panels;
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

        // CreateSections(services);

        services.AddSingleton<IShell, Zafiro.UI.Shell.Shell>();
        services.AddSingleton(DialogService.Create());
        services.AddSingleton(NotificationService.Instance);
        services.RegisterAllViewModels(typeof(MainViewModel).Assembly);
        services.RegisterAllSections(typeof(MainViewModel).Assembly);
        services.AddTransient<MainViewModel>();
        // services.AddScoped<SlimDataGridViewModel>();
        // services.AddScoped<NumberBoxBehaviorViewModel>();
        // services.AddScoped<WizardViewModel>();
        // services.AddScoped<TypewriterViewModel>();
        // services.AddScoped<DiagramsViewModel>();
        // services.AddScoped<EnhancedDiagramViewModel>();
        // services.AddScoped<DragViewModel>();
        // services.AddScoped<DialogSampleViewModel>();

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider.GetRequiredService<MainViewModel>();
    }

    private static void CreateSections(ServiceCollection services)
    {
        services.RegisterSections(builder => builder
            .Add<SlimDataGridViewModel>("SlimDataGrid", new IconViewModel { IconId = "fa-wallet" })
            .Add<NumberBoxBehaviorViewModel>("NumbersOnlyTextBox", new IconViewModel { IconId = "mdi-numeric" })
            .Add<WizardViewModel>("SlimWizard", new IconViewModel { IconId = "mdi-wizard-hat" })
            .Add<TypewriterViewModel>("Typewriter", new IconViewModel { IconId = "mdi-typewriter" })
            .Add<DiagramsViewModel>("Diagrams", new IconViewModel { IconId = "mdi-numeric" })
            //.Add<EnhancedDiagramViewModel>("Diagrams Enhanced", new IconViewModel{ IconId = "fa-diagram-project" })
            .Add<DragViewModel>("Drag", new IconViewModel { IconId = "fa-diagram-project" })
            .Add<DialogSampleViewModel>("Dialogs", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<CircularProgressBarViewModel>("CircularProgressBar", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<GradientControlViewModel>("GradientControl", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<ProportionalCanvasViewModel>("ProportionalCanvas", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<GraphViewModel>("Graph", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<GradualGraphViewModel>("GradualGraph", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<NonOverlappingCanvasSectionViewModel>("NonOverlappingCanvas", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<TableViewModel>("Table", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<DendrogramViewModel>("Dendrogram", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<HeatmapWithDendrogramsViewModel>("HeatmapWithDendrograms", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<HeatmapViewModel>("Heatmap", new IconViewModel { IconId = "fa-comment-dots" })
            .Add<SamplerViewModel>("Sampler", new IconViewModel { IconId = "fa-comment-dots" })
        );
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