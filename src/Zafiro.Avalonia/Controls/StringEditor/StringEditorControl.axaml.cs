using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.Primitives;
using Zafiro.UI.Fields;

namespace Zafiro.Avalonia.Controls.StringEditor;

public class StringEditorControl : TemplatedControl
{
    public static readonly StyledProperty<Field<string>> FieldProperty = AvaloniaProperty.Register<StringEditorControl, Field<string>>(
        nameof(Field<string>));
    
    public static readonly StyledProperty<bool> IsEditingProperty = AvaloniaProperty.Register<StringEditorControl, bool>(nameof(IsEditing), defaultValue: false);
    public static readonly StyledProperty<ICommand> EditProperty = AvaloniaProperty.Register<StringEditorControl, ICommand>(nameof(Edit));

    public Field<string> Field
    {
        get => GetValue(FieldProperty);
        set => SetValue(FieldProperty, value);
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

    public static readonly StyledProperty<bool> IsLockedProperty = AvaloniaProperty.Register<StringEditorControl, bool>(
        nameof(IsLocked), defaultValue: true);

    public bool IsLocked
    {
        get => GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }
}