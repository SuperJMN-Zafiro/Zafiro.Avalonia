using System.Reactive;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls.StringEditor;

public class Editable<T> : ReactiveObject, IModelWrapper where T : IModel
{
    public Editable(T initial, Func<T> factory, Action<T, T> copyTo)
    {
        Instance = initial;
        WorkInstance = factory();
        copyTo(initial, (T) WorkInstance);
        Commit = ReactiveCommand.Create(() =>
        {
            copyTo((T) WorkInstance, (T) Instance);
        }, WorkInstance.IsValid);
        Cancel = ReactiveCommand.Create(() => copyTo((T)Instance, (T)WorkInstance));
    }

    public IReactiveCommand<Unit, Unit> Cancel { get; }

    public IReactiveCommand<Unit, Unit> Commit { get; }

    public IModel WorkInstance { get; }
    public IModel Instance { get; }

    public T InstanceOfType => (T) Instance;
    public T WorkInstanceOfType => (T) WorkInstance;
}