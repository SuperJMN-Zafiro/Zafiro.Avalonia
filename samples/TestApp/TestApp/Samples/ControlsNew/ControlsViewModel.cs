using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.ControlsNew.Navigation;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.ControlsNew.Wizard;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Shell.Sections;
using Zafiro.UI.Navigation;
using Zafiro.UI.Navigation.Zafiro.UI.Navigation;
using Section = Zafiro.Avalonia.Shell.Sections.Section;

namespace TestApp.Samples.ControlsNew
{
    public class ControlsViewModel
    {
        private readonly IServiceProvider serviceProvider;

        public ControlsViewModel()
        {
            var serviceCollection = new ServiceCollection();
            
            // Register the ServiceProviderTypeResolver factory
            serviceCollection.AddScoped<ITypeResolver>(provider => 
                new ServiceProviderTypeResolver(provider));
                
            // Register the Navigator with the resolver
            serviceCollection.AddScoped<INavigator>(provider => 
                new Navigator(provider.GetRequiredService<ITypeResolver>()));
            
            // Register your view models
            serviceCollection.AddScoped<NavigationSampleViewModel>();
            serviceCollection.AddScoped<TargetViewModel>();
            
            // Build the provider
            serviceProvider = serviceCollection.BuildServiceProvider();
            
            Sections = new List<SectionBase>
            {
                Section.Create("Typewriter", () => new TypewriterViewModel()),
                Section.Create("DataGrid", () => new SlimDataGridViewModel()),
                Section.Create("Wizard", () => new WizardViewModel(DialogService.Create())),
                CreateNavigation<NavigationSampleViewModel>("Navigation")
            };
        }

        private SectionBase CreateNavigation<T>(string name) where T : notnull
        {
            return Section.Create(name, () => {
                // Create a new NavigationHost with a scoped resolver
                return new NavigationHost(
                    // Factory to create a scoped resolver
                    () => new ScopedTypeResolver(serviceProvider),
                    
                    // Factory to create initial content using the resolver
                    resolver => resolver.Resolve<T>()
                );
            });
        }

        public List<SectionBase> Sections { get; }
    }
}