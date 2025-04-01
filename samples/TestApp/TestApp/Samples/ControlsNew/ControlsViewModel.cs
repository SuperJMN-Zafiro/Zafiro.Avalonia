using System;
using System.Collections.Generic;
using System.Linq;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
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
            serviceCollection.AddScoped<INavigator>(provider => new Navigator(provider, Maybe<ILogger>.None));
            
            return serviceCollection.AddSingleton<IEnumerable<SectionBase>>(provider => new List<SectionBase>
            {
                CreateSection<TypewriterViewModel>("Typewriter", provider),
                CreateSection<SlimDataGridViewModel>("SlimDataGrid", provider),
                CreateSection<WizardViewModel>("Wizard", provider),
                CreateSection<NavigationSampleViewModel>("Navigation", provider)
            });
        }

        private static SectionBase CreateSection<T>(string name, IServiceProvider provider) where T : notnull
        {
            return Section.Create(name, () => new SectionScope(provider, typeof(T)));
        }

        public List<SectionBase> Sections { get; }
    }
}