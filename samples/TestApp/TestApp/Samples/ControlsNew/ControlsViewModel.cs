using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.Samples.ControlsNew.Navigation;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.ControlsNew.Wizard;
using Zafiro.Avalonia.Dialogs;
using Zafiro.UI;
using Zafiro.UI.Navigation;

namespace TestApp.Samples.ControlsNew;

public class ControlsViewModel
{
    public ControlsViewModel(IDialog dialog)
    {
        var navigator = new Navigator(Maybe<ITypeResolver>.None);
        
        Sections = new List<Section>
        {
            new("Typewriter", new TypewriterViewModel(), Maybe<object>.None),
            new("DataGrid", new SlimDataGridViewModel(), Maybe<object>.None),
            new("Wizard", new WizardViewModel(dialog), Maybe<object>.None),
            new("Navigation", new NavigationViewModel(navigator, () => new NavigationSampleViewModel(navigator)), Maybe<object>.None),
        };
    }

    public IEnumerable<ISection> Sections { get; }
}