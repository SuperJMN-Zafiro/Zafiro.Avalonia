using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.Samples.Diagrams.Enhanced;
using TestApp.Samples.Diagrams.Simple;
using Zafiro.UI;
using Zafiro.UI.Shell.Utils;

namespace TestApp.Samples.Diagrams;

[Section("fa-diagram-project")]
public class DiagramsViewModel
{
    public DiagramsViewModel()
    {
        Sections = new List<SectionOld>
        {
            new("Regular", new SimpleDiagramViewModel(), Maybe<object>.None),
            new("Enhanced", new EnhancedDiagramViewModel(), Maybe<object>.None),
        };
    }

    public IEnumerable<ISectionOld> Sections { get; }
}