using ReactiveUI.SourceGenerators;
using Zafiro.UI.Navigation.Sections;

namespace Zafiro.Avalonia.Controls.Shell;

public partial class ContentSectionDesign : ReactiveObject, IContentSection
{
    [Reactive] private object? icon;

    public ContentSectionDesign()
    {
        SortOrder = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(10)).Select(l => Random.Shared.Next(30));
    }

    public ContentSectionDesign(string name, object? icon = null, bool isPrimary = true)
    {
        Name = name;
        Icon = icon;
        IsPrimary = isPrimary;
        SortOrder = Observable.Timer(DateTimeOffset.Now, TimeSpan.FromSeconds(10)).Select(l => Random.Shared.Next(30));
    }

    public bool IsPrimary { get; init; } = true;
    public string Name { get; set; }
    public string FriendlyName { get; set; }
    public IObservable<bool> IsVisible { get; init; } = Observable.Return(true);
    public IObservable<int> SortOrder { get; init; }
    public IObservable<object> Content { get; set; }
    public Type RootType { get; } = typeof(object);
}