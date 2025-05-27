using Zafiro.UI.Navigation.Sections;
using Zafiro.UI.Shell;

namespace Zafiro.Avalonia.Controls.Shell;

public class ShellDesign : IShell
{
    public void GoToSection(string sectionName)
    {
        throw new NotSupportedException();
    }

    public object Header { get; set; } = "Header that is too long to fit in the header";
    public IObservable<object> ContentHeader { get; set; } = Observable.Return("Content Header");

    public IEnumerable<ISection> Sections =>
    [
        new ContentSectionDesign { Name = "Hi Test section 1. Very long for the testing", Icon = new Icon() { Source = "fa-wallet", }, Content = Observable.Return("Test") },
        new ContentSectionDesign { Name = "Test section 2", Icon = new Icon() { Source = "fa-gear" } },
        new ContentSectionDesign { Name = "Test section 3", Icon = new Icon() { Source = "fa-user" } },
        new CommandSectionDesign { Name = "Test section 3", Icon = new Icon() { Source = "fa-user" } },
    ];

    public IContentSection SelectedSection { get; set; }
}