using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public class SectionDesign : IContentSection
{
    public bool IsPrimary { get; init; } = true;
    public string Name { get; set; }
    public object? Icon { get; set; }
    public IObservable<object> Content => Observable.Return("DESIGN TIME");
}