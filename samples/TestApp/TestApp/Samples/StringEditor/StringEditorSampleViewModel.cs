using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Controls.StringEditor;
using Zafiro.UI.Fields;

namespace TestApp.Samples.StringEditor;

public class StringEditorSampleViewModel : ReactiveValidationObject
{
    public StringField StringField { get; }

    public StringEditorSampleViewModel()
    {
        StringField = new("Hola");
        StringField.AddRule(s => s is { Length: < 7 }, "Too long");
    }
}