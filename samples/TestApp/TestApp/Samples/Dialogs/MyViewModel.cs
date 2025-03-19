using System.Reactive;
using ReactiveUI;
using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;
using Zafiro.Avalonia.Dialogs;

namespace TestApp.Samples.Dialogs;

public partial class MyViewModel : ReactiveValidationObject
{
    [Reactive] private string text;

    public MyViewModel(IDialog dialogService)
    {
        Text = "";
        OpenAnotherDialog = ReactiveCommand.CreateFromTask(() => dialogService.ShowMessage("Another dialog", "This is another dialog"));
        this.ValidationRule(x => x.Text, s =>  !string.IsNullOrEmpty(s), "Can't be empty");
    }

    public ReactiveCommand<Unit,Unit> OpenAnotherDialog { get; }
}