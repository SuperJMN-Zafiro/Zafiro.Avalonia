using System.Collections.Generic;
using CSharpFunctionalExtensions;
using TestApp.ViewModels;
using Zafiro.UI;
using Section = Zafiro.UI.Section;

namespace TestApp.Samples.Panels;

public class PanelsSectionViewModel : ViewModelBase, ISectionNode
{
    public PanelsSectionViewModel()
    {
        Sections = [new Section("NonOverlappingCanvas", new NonOverlappingCanvasSectionViewModel(), Maybe<object>.None)];
    }
    
    public IEnumerable<ISection> Sections { get; set; }
}

public interface ISectionNode
{
    public IEnumerable<ISection> Sections { get; set; }
}