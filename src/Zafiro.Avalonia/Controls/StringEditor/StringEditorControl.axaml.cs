using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls.StringEditor;

[TemplatePart("PART_TextBox", typeof(TextBox))]
public class StringEditorControl : TemplatedControl
{
    public static readonly StyledProperty<StringField> StringFieldProperty = AvaloniaProperty.Register<StringEditorControl, StringField>(
        nameof(StringField));
    
    public static readonly StyledProperty<bool> IsEditingProperty = AvaloniaProperty.Register<StringEditorControl, bool>(nameof(IsEditing), defaultValue: false);
    public static readonly StyledProperty<ICommand> EditProperty = AvaloniaProperty.Register<StringEditorControl, ICommand>(nameof(Edit));

    public StringField StringField
    {
        get => GetValue(StringFieldProperty);
        set => SetValue(StringFieldProperty, value);
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

    private TextBox? textBox;

    public bool IsLocked
    {
        get => GetValue(IsLockedProperty);
        set => SetValue(IsLockedProperty, value);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        textBox = e.NameScope.Find<TextBox>("PART_TextBox");
        base.OnApplyTemplate(e);
    }
}