using ReactiveUI.SourceGenerators;

namespace Zafiro.Avalonia;

public partial class IconViewModel : ReactiveObject
{
    [Reactive] private string iconId;
}