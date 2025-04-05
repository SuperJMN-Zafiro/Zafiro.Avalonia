using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.ViewModels;
using Zafiro.UI;

namespace TestApp.Samples.Panels;

public class PanelsSectionViewModel : ViewModelBase, ISectionNode
{
    public PanelsSectionViewModel()
    {
        Sections = [new SectionOld("NonOverlappingCanvas", new NonOverlappingCanvasSectionViewModel(), Maybe<object>.None)];
    }
    
    public IEnumerable<ISectionOld> Sections { get; set; }
}

public interface ISectionNode
{
    public IEnumerable<ISectionOld> Sections { get; set; }
}