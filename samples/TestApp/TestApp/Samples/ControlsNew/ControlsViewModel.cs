using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.ControlsNew.Loading;
using TestApp.Samples.ControlsNew.Navigation;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.ControlsNew.Wizard;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Services;
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
            serviceCollection.AddScoped<SlimWizard.WizardViewModel>();
            serviceCollection.AddScoped<LoadingSampleViewModel>();

            serviceCollection.AddSingleton(DialogService.Create());
            serviceCollection.AddSingleton(NotificationService.Instance);

            serviceCollection.RegisterSections(sections =>
            {
                sections
                    .Add<TypewriterViewModel>("Typewriter", null)
                    .Add<SlimDataGridViewModel>("Slim DataGrid", null)
                    .Add<WizardViewModel>("Wizard", null)
                    .Add<SlimWizard.WizardViewModel>("Slim Wizard", null)
                    .Add<NavigationSampleViewModel>("Navigation", null)
                    .Add<LoadingSampleViewModel>("Loading", null);
            });

            var buildServiceProvider = serviceCollection.BuildServiceProvider();
            var requiredService = buildServiceProvider.GetRequiredService<IEnumerable<ISection>>();
            Sections = requiredService.ToList();
        }

        public List<ISection> Sections { get; }
    }
}