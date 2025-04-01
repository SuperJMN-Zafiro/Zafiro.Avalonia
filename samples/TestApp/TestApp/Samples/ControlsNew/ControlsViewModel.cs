using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
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
        public ControlsViewModel()
        {
            var serviceCollection = new ServiceCollection();
            
            // Register your view models
            serviceCollection.AddScoped<NavigationSampleViewModel>();
            serviceCollection.AddScoped<TargetViewModel>();
            serviceCollection.AddScoped<TypewriterViewModel>();
            serviceCollection.AddScoped<SlimDataGridViewModel>();
            serviceCollection.AddScoped<WizardViewModel>();
            serviceCollection.AddSingleton<IDialog>(DialogService.Create());
            
            RegisterSections(serviceCollection);

            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            var requiredService = buildServiceProvider.GetRequiredService<IEnumerable<SectionBase>>();
            Sections = requiredService.ToList();
        }

        private IServiceCollection RegisterSections(IServiceCollection serviceCollection)
        {
            // Register the ServiceProviderTypeResolver factory
            serviceCollection.AddScoped<ITypeResolver>(provider => 
                new ServiceProviderTypeResolver(provider));
                
            // Register the Navigator with the resolver
            serviceCollection.AddScoped<INavigator>(provider => 
                new Navigator(provider.GetRequiredService<ITypeResolver>()));
            
            return serviceCollection.AddSingleton<IEnumerable<SectionBase>>(provider => new List<SectionBase>
            {
                CreateWithoutNavigation<TypewriterViewModel>("Typewriter", provider),
                CreateWithoutNavigation<SlimDataGridViewModel>("SlimDataGrid", provider),
                CreateWithoutNavigation<WizardViewModel>("Wizard", provider),
                CreateNavigation<NavigationSampleViewModel>("Navigation", provider)
            });
        }

        private static SectionBase CreateNavigation<T>(string name, IServiceProvider provider) where T : notnull
        {
            return Section.Create(name, () => {
                // Create a new NavigationHost with a scoped resolver
                return new NavigationHost(
                    // Factory to create a scoped resolver
                    () => new ScopedTypeResolver(provider),
                    
                    // Factory to create initial content using the resolver
                    resolver => resolver.Resolve<T>()
                );
            });
        }
        
        private SectionBase CreateWithoutNavigation<T>(string name, IServiceProvider serviceProvider) where T : notnull
        {
            return Section.Create(name,() =>  serviceProvider.GetRequiredService<T>());
        }

        public List<SectionBase> Sections { get; }
    }
}