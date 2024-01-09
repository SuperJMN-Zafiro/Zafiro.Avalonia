using ReactiveUI.Validation.Helpers;
using Zafiro.UI.Fields;

namespace TestApp.Samples.StringEditor;

public class StringEditorSampleViewModel : ReactiveValidationObject
{
    public Field<string> Field { get; }

    public StringEditorSampleViewModel()
    {
        Field = new("Hola");
        Field.Validate(s => s is { Length: < 7 }, "Too long");
    }
}