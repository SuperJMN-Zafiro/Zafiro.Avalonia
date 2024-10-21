using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.Avalonia;

namespace ClassLibrary1;

public partial class Selectable : ReactiveObject, ISelectable
{
    [Reactive] private bool isSelected;
}