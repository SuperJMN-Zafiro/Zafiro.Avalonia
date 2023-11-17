using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using ReactiveUI;

namespace Zafiro.Avalonia.Controls.StringEditor;

public class StringEditorControl : TemplatedControl
{
    public static readonly StyledProperty<EditableString> WrapperProperty = AvaloniaProperty.Register<StringEditorControl, EditableString>(
        nameof(Wrapper));
    
    public static readonly StyledProperty<bool> IsEditingProperty = AvaloniaProperty.Register<StringEditorControl, bool>(nameof(IsEditing), defaultValue: false);
    public static readonly StyledProperty<ICommand> EditProperty = AvaloniaProperty.Register<StringEditorControl, ICommand>(nameof(Edit));

    public StringEditorControl()
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