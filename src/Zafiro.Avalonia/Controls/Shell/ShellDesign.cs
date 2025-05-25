using Zafiro.UI.Navigation.Sections;
using Zafiro.UI.Shell;

namespace Zafiro.Avalonia.Controls.Shell;

public class ShellDesign : IShell
{
    public void GoToSection(string sectionName)
    {
        throw new NotSupportedException();
    }

    public IEnumerable<ISection> Sections =>
    [
        new SectionDesign { Name = "Test section 1", Icon = new Icon() { IconId = "fa-wallet" } },
        new SectionDesign { Name = "Test section 2", Icon = new Icon() { IconId = "fa-wallet" } },
        new SectionDesign { Name = "Test section 3", Icon = new Icon() { IconId = "fa-wallet" } }
    ];

    public IContentSection SelectedSection { get; set; }
}