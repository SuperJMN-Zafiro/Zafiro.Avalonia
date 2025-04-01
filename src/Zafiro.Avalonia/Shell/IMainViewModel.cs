using Zafiro.UI.Navigation.Sections;
using Section = Zafiro.UI.Navigation.Sections.Section;

namespace Zafiro.Avalonia.Shell;

public interface IMainViewModel
{
    IEnumerable<Section> Sections { get; }
    IContentSection SelectedSection { get; set; }
    void GoToSection(string sectionName);
}