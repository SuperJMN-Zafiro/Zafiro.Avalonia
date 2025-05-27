using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public class ContentSectionDesign : IContentSection
{
    public ContentSectionDesign()
    {
    }

    public ContentSectionDesign(string name, object? icon = null, bool isPrimary = true)
    {
        Name = name;
        Icon = icon;
        IsPrimary = isPrimary;
    }

    public bool IsPrimary { get; init; } = true;
    public string Name { get; set; }
    public object? Icon { get; set; }
    public IObservable<object> Content { get; set; }
}