using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.StringEditor;

namespace TestApp.Samples.StringEditor;

public class StringEditorSampleViewModel : ReactiveValidationObject
{
    public EditableString EditableString { get; }

    public StringEditorSampleViewModel()
    {
        EditableString = new("Hola");
        EditableString.ValidationRule(s => s is { Length: < 7 }, "Too long");
    }
}