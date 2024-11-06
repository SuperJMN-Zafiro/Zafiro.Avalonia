using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace TestApp.Samples.Dialogs;

public partial class MyViewModel : ReactiveValidationObject
{
    [Reactive] private string text;

    public MyViewModel()
    {
        Text = "";
        this.ValidationRule(x => x.Text, s =>  !string.IsNullOrEmpty(s), "Can't be empty");
    }
}