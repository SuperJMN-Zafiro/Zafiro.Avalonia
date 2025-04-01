using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.ControlsNew.Navigation;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.ControlsNew.Wizard;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Shell;
using Zafiro.Avalonia.Shell.Sections;
using Zafiro.UI.Navigation;
using Zafiro.UI.Navigation.Zafiro.UI.Navigation;
using Section = Zafiro.Avalonia.Shell.Sections.Section;

namespace TestApp.Samples.ControlsNew;

public class ControlsViewModel
{
    public ControlsViewModel()
    {
        var serviceCollection = new ServiceCollection();
        
        // Register your view models first
        serviceCollection.AddScoped<NavigationSampleViewModel>();
        serviceCollection.AddScoped<IOtherDependency, OtherDependency>();
        
        // Build the initial provider
        var provider = serviceCollection.BuildServiceProvider();
        
        // Create a ServiceProviderTypeResolver with parameter support
        var typeResolver = new ServiceProviderTypeResolver(provider);
        
        // Register any custom factories if needed
        // Example: typeResolver.RegisterFactory<DetailViewModel, string>((sp, id) => new DetailViewModel(id, sp.GetService<IOtherDependency>()));
        typeResolver.RegisterFactory<TargetViewModel, string>((sp, id) => new TargetViewModel(id, sp.GetRequiredService<IOtherDependency>()));
        
        // Register the Navigator with the resolver
        serviceCollection.AddScoped<INavigator>(serviceProvider => new Navigator(typeResolver));
        
        // Build the final provider with the Navigator
        provider = serviceCollection.BuildServiceProvider();
        
        Sections = new List<SectionBase>
        {
            Section.Create("Typewriter", () => new TypewriterViewModel()),
            Section.Create("DataGrid", () => new SlimDataGridViewModel()),
            Section.Create("Wizard", () => new WizardViewModel(DialogService.Create())),
            CreateNavigation<NavigationSampleViewModel>("Navigation", provider),
        };
    }

    private SectionBase CreateNavigation<T>(string name, ServiceProvider provider) where T : notnull
    {
        return Section.Create(name, () => new NavigationHost(provider, serviceProvider => 
            serviceProvider.GetRequiredService<T>()));
    }

    public List<SectionBase> Sections { get; }
}

public class OtherDependency : IOtherDependency
{
}