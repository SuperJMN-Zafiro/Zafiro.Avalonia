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
        new SectionDesign { Name = "Test section 1", Icon = new IconViewModel() { IconId = "fa-wallet" } },
        new SectionDesign { Name = "Test section 2", Icon = new IconViewModel() { IconId = "fa-wallet" } },
        new SectionDesign { Name = "Test section 3", Icon = new IconViewModel() { IconId = "fa-wallet" } }
    ];

    public IContentSection SelectedSection { get; set; }
}

public class SectionDesign : IContentSection
{
    public Func<object?> GetViewModel { get; } = () => new object();
    public bool IsPrimary { get; init; } = true;
    public string Name { get; set; }
    public object? Icon { get; set; }
    public IObservable<object> Content => Observable.Return("DESIGN TIME");
}