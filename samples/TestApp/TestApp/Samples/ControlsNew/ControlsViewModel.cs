using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Samples.ControlsNew.Navigation;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.ControlsNew.Wizard;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Shell;
using Zafiro.Avalonia.Shell.Sections;
using Zafiro.UI.Navigation;
using Section = Zafiro.Avalonia.Shell.Sections.Section;

namespace TestApp.Samples.ControlsNew;

public class ControlsViewModel
{
    public ControlsViewModel()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddScoped<INavigator>(serviceProvider => new Navigator(new TypeResolver(serviceProvider)));
        serviceCollection.AddScoped<NavigationSampleViewModel>();
        var provider = serviceCollection.BuildServiceProvider();
        
        Sections = new List<SectionBase>
        {
            Section.Create("Typewriter", () => new TypewriterViewModel()),
            Section.Create("DataGrid", () => new SlimDataGridViewModel()),
            Section.Create("Wizard", () => new WizardViewModel(DialogService.Create())),
            CreateNavigation<NavigationSampleViewModel>("Navigation", provider),
        };
    }

    private SectionBase CreateNavigation<T>(string name, ServiceProvider provider)
    {
        return Section.Create(name, () =>  new NavigationViewModel(provider, serviceProvider => serviceProvider.GetRequiredService<T>())) ;
    }

    public List<SectionBase> Sections { get; }
}

public class TypeResolver(IServiceProvider serviceProvider) : ITypeResolver
{
    [return: NotNull]
    public T Resolve<T>()
    {
        throw new NotImplementedException();
    }
}