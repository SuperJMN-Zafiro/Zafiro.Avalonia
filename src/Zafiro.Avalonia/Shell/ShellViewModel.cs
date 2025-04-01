using ReactiveUI.SourceGenerators;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Shell;

public partial class ShellViewModel : ReactiveObject, IMainViewModel
{
    [Reactive] private IContentSection? selectedSection;

    public ShellViewModel(IEnumerable<SectionBase> sections)
    {
        Sections = sections;
        CurrentContent = this.WhenAnyValue<ShellViewModel, IContentSection>(x => x.SelectedSection)
            .WhereNotNull()
            .Select(section => section.GetViewModel());
        SelectedSection = Sections.OfType<IContentSection>().FirstOrDefault();
    }

    public IEnumerable<SectionBase> Sections { get; }
    public IObservable<object?> CurrentContent { get; }

    public void GoToSection(string sectionName)
    {
        SelectedSection = Sections.OfType<IContentSection>().First(x => x.Name == sectionName);
    }
}