using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using ReactiveUI.Validation.Extensions;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Dialogs.Simple;
using Zafiro.Avalonia.MigrateToZafiro;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace TestApp.Samples.Dialogs;

public class DialogSampleViewModel : IViewModel
{
    public DialogSampleViewModel(INotificationService notificationService, ISimpleDialog dialogService)
    {
        ShowDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            var myViewModel = new MyViewModel();
            return await dialogService.ShowAndGetResult(myViewModel, "Dale durity", myViewModel.IsValid(),
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

        ShowMessage= ReactiveCommand.CreateFromTask(() => OnShowMessage(dialogService));
    }

    private static Task OnShowMessage(ISimpleDialog dialogService)
    {
        return dialogService.ShowMessage("Dialog Title", "Hi, this is the text of the dialog. The View is connected to the ViewModel using a DataTemplate", "Dismiss");
    }

    public ReactiveCommand<Unit, Unit> ShowMessage { get; set; }

    public ReactiveCommand<Unit, Maybe<string>> ShowDialog { get; set; }
}