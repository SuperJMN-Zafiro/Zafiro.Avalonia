using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls.StringEditor;

public class StringEditor : TemplatedControl
{
    public static readonly StyledProperty<EditableString> WrapperProperty = AvaloniaProperty.Register<StringEditor, EditableString>(
        nameof(Wrapper));
    
    public static readonly StyledProperty<bool> IsEditingProperty = AvaloniaProperty.Register<StringEditor, bool>(nameof(IsEditing), defaultValue: false);
    public static readonly StyledProperty<ICommand> EditProperty = AvaloniaProperty.Register<StringEditor, ICommand>(nameof(Edit));

    public StringEditor()
    {
        Edit = ReactiveCommand.Create(() => IsEditing = true);
        this.WhenAnyValue(x => x.Wrapper)
            .WhereNotNull()
            .Subscribe(wrapper =>
        {
            wrapper.Commit.Merge(wrapper.Cancel).Do(_ => IsEditing = false).Subscribe();
        });

        this.WhenAnyValue(x => x.IsEditing).Do(isEditing =>
        {
            if (isEditing)
            {
                PseudoClasses.Set(":editing", true);
            } else
            {
                PseudoClasses.Set(":editing", false);
            }
        }).Subscribe();
    }
    
    public Editable<StringBox> Wrapper
    {
        get => GetValue(WrapperProperty);
        set => SetValue(WrapperProperty, value);
    }
    
    public bool IsEditing
    {
        get => GetValue(IsEditingProperty);
        set => SetValue(IsEditingProperty, value);
    }

    public ICommand Edit
    {
        get => GetValue(EditProperty);
        set => SetValue(EditProperty, value);
    }
}

public class EditableString : Editable<StringBox>
{
    public EditableString(string initialValue) : base(new StringBox(initialValue), () => new StringBox(initialValue), (a, b) => b.Text = a.Text )
    {
    }
}

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

public interface IModelWrapper
{
    IReactiveCommand<Unit, Unit> Cancel { get; }
    IReactiveCommand<Unit, Unit> Commit { get; }
    IModel WorkInstance { get;  }
    IModel Instance { get; }
}