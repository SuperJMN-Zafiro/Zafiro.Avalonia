using ReactiveUI.SourceGenerators;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public partial class ContentSectionDesign : ReactiveObject, IContentSection
{
    [Reactive] private object? icon;

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

    public IObservable<object> Content { get; set; }
}