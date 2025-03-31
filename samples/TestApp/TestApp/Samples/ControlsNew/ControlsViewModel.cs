using System.Collections.Generic;
using TestApp.Samples.ControlsNew.Navigation;
using TestApp.Samples.ControlsNew.SlimDataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using TestApp.Samples.ControlsNew.Wizard;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Shell;
using Zafiro.Avalonia.Shell.Sections;
using Section = Zafiro.Avalonia.Shell.Sections.Section;

namespace TestApp.Samples.ControlsNew;

public class ControlsViewModel
{
    public ControlsViewModel()
    {
        Sections = new List<SectionBase>
        {
            Section.Create("Typewriter", () => new TypewriterViewModel()),
            Section.Create("DataGrid", () => new SlimDataGridViewModel()),
            Section.Create("Wizard", () => new WizardViewModel(DialogService.Create())),
            //Section.Create("Navigation", () => new NavigationSampleViewModel()),
        };
    }

    public List<SectionBase> Sections { get; }
}