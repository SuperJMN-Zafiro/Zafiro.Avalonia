using System;
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
using Section = Zafiro.Avalonia.Shell.Sections.Section;

namespace TestApp.Samples.ControlsNew;

public class ControlsViewModel
{
    public ControlsViewModel()
    {
        var serviceCollection = new ServiceCollection();
        
        // Register the TypeResolver that works with the same pattern you already had
        // Este enfoque mantiene la compatibilidad con tu código existente
        serviceCollection.AddScoped<INavigator>(serviceProvider => 
            new Navigator(new TypeResolver(serviceProvider)));
            
        // Register your view models
        serviceCollection.AddScoped<NavigationSampleViewModel>();
        
        // Build the provider AFTER all registrations
        var provider = serviceCollection.BuildServiceProvider();
        
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

// Esta es la clase TypeResolver original, pero actualizada para implementar ITypeWithParametersResolver
public class TypeResolver : ITypeResolver, ITypeWithParametersResolver
{
    private readonly IServiceProvider serviceProvider;

    public TypeResolver(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public T Resolve<T>() where T : notnull
    {
        return serviceProvider.GetRequiredService<T>();
    }

    public T Resolve<T, TParam>(TParam parameter) where T : notnull
    {
        // Esta implementación simple busca un constructor que acepte el parámetro
        // Si no lo encuentra, intenta resolver el tipo normalmente
        var type = typeof(T);
        var constructors = type.GetConstructors();
        
        foreach (var constructor in constructors)
        {
            var parameters = constructor.GetParameters();
            if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(typeof(TParam)))
            {
                // Necesitamos resolver las otras dependencias del constructor
                if (parameters[0].ParameterType == typeof(TParam))
                {
                    return (T)constructor.Invoke(new object[] { parameter });
                }
            }
        }
        
        // Si no encontramos un constructor adecuado, intentamos resolver normalmente
        // aunque esto probablemente fallará si el tipo realmente necesita el parámetro
        return serviceProvider.GetRequiredService<T>();
    }
}