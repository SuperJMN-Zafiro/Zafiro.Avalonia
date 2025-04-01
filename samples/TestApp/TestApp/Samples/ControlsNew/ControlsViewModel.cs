using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.ControlsNew.Navigation;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.ControlsNew.Wizard;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI.Navigation;
using Zafiro.UI.Navigation.Sections;

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
            serviceCollection.AddSingleton(DialogService.Create());

            serviceCollection.RegisterSections(sections =>
            {
                sections
                    .Add<TypewriterViewModel>("Typewriter")
                    .Add<SlimDataGridViewModel>("Slim DataGrid")
                    .Add<WizardViewModel>("Wizard")
                    .Add<NavigationSampleViewModel>("Navigation");
            });

            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            var requiredService = buildServiceProvider.GetRequiredService<IEnumerable<SectionBase>>();
            Sections = requiredService.ToList();
        }
        
        public List<SectionBase> Sections { get; }
    }
}