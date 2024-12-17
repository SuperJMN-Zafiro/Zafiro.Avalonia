using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.ControlsNew.Wizards;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.UI;

namespace TestApp.Samples.ControlsNew;

public class ControlsViewModel
{
    public ControlsViewModel(IDialog dialog)
    {
        Sections = new List<Section>
        {
            new("Typewriter", new TypewriterViewModel(), Maybe<object>.None),
            new("DataGrid", new SlimDataGridViewModel(), Maybe<object>.None),
            new("Wizard", new WizardViewModel(dialog), Maybe<object>.None),
        };
    }

    public IEnumerable<ISection> Sections { get; }
}