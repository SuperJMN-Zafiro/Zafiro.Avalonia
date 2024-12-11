using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.Samples.ControlsNew.DataGrid;
using TestApp.Samples.ControlsNew.Typewriter;
using Zafiro.UI;

namespace TestApp.Samples.ControlsNew;

public class ControlsViewModel
{
    public ControlsViewModel()
    {
        Sections = new List<Section>
        {
            new("Typewriter", new TypewriterViewModel(), Maybe<object>.None),
            new("DataGrid", new DataGridViewModel(), Maybe<object>.None),
        };
    }

    public IEnumerable<ISection> Sections { get; }
}