using System.Reactive;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls.StringEditor;

public interface IModelWrapper
{
    IReactiveCommand<Unit, Unit> Cancel { get; }
    IReactiveCommand<Unit, Unit> Commit { get; }
    IModel WorkInstance { get;  }
    IModel Instance { get; }
}