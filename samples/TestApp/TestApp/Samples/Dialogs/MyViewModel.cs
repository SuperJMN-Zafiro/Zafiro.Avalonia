using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro;
using Zafiro.UI;

namespace TestApp.Samples.Dialogs;

public class MyViewModel : ReactiveValidationObject
{
    [Reactive]
    public string Text { get; set; }

    public MyViewModel()
    {
        Text = "";
        this.ValidationRule(x => x.Text, s =>  !string.IsNullOrEmpty(s), "Can't be empty");
    }
}