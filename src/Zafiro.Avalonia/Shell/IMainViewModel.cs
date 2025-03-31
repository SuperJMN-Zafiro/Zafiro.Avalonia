using Zafiro.Avalonia.Shell.Sections;

namespace Zafiro.Avalonia.Shell;

public interface IMainViewModel
{
    IEnumerable<SectionBase> Sections { get; }
    IContentSection SelectedSection { get; set; }
    void GoToSection(string sectionName);
}