using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.MigrateToZafiro;
using Zafiro.Avalonia.Misc;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace TestApp.Samples.Dialogs;

public class DialogSampleViewModel : IViewModel
{
    public DialogSampleViewModel(INotificationService notificationService, IDialogService dialogService)
    {
        var dialog = new DesktopDialogService(Maybe<Action<ConfigureWindowContext>>.None);
        ShowDialog = ReactiveCommand.CreateFromTask(() =>
        {
            return dialog.ShowDialog(new MyViewModel(), "Dale durity", model => Observable.FromAsync(() => model.Result), new OptionConfiguration<MyViewModel, string>("OK", x => ReactiveCommand.Create(() => x.SetResult(x.Text))));
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

    private static Task OnShowMessage(IDialogService dialogService)
    {
        return dialogService.ShowMessage("Dismiss", "Dialog Title", "Hi, this is the text of the dialog. The View is connected to the ViewModel using a DataTemplate");
    }

    public ReactiveCommand<Unit, Unit> ShowMessage { get; set; }

    public ReactiveCommand<Unit, Maybe<string>> ShowDialog { get; set; }
}