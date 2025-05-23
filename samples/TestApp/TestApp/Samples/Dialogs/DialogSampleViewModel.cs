using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using TestApp.Samples.Adorners;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.MigrateToZafiro;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace TestApp.Samples.Dialogs;

[Icon("fa-comment-dots")]
public class DialogSampleViewModel : IViewModel
{
    public DialogSampleViewModel(INotificationService notificationService, IDialog dialogService)
    {
        ShowDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            return await dialogService.ShowAndGetResult(new MyViewModel(dialogService), "Dale durity", model => model.IsValid(),
                model => model.Text);
        });

        ShowDialog
            .Values()
            .SelectMany(x => TaskMixin.ToSignal(() => notificationService.Show($"You entered \"{x}\" in the dialog ;)", "Howdy!")))
            .Subscribe();

        ShowDialog
            .Empties()
            .SelectMany(_ => TaskMixin.ToSignal(() => notificationService.Show("You dismissed the dialog", "Ouch!")))
            .Subscribe();

        ShowMessage = ReactiveCommand.CreateFromTask(() => OnShowMessage(dialogService));
    }

    public ReactiveCommand<Unit, Unit> ShowMessage { get; set; }

    public ReactiveCommand<Unit, Maybe<string>> ShowDialog { get; set; }

    private static Task OnShowMessage(IDialog dialogService)
    {
        return dialogService.ShowMessage("Dialog Title", "Hi, this is the text of the dialog. The View is connected to the ViewModel using a DataTemplate");
    }
}