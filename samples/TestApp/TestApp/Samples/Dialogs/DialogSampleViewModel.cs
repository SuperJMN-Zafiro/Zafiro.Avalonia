using System;
using System.Reactive;
using System.Reactive.Linq;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Misc;
using Zafiro.Avalonia.Mixins;
using Zafiro.CSharpFunctionalExtensions;
using Zafiro.UI;

namespace TestApp.Samples.Dialogs;

public class DialogSampleViewModel : IViewModel
{
    public DialogSampleViewModel(INotificationService notificationService)
    {
        var dialog = new NewDialogService(Maybe<Action<ConfigureWindowContext>>.None);
        ShowDialog = ReactiveCommand.CreateFromTask(() =>
        {
            return dialog.ShowDialog(new MyViewModel(), "Dale durity", model => Observable.FromAsync(() => model.Result), new NewOptionConfiguration<MyViewModel, string>("OK", x => ReactiveCommand.Create(() => x.SetResult(x.Text))));
        });

        ShowDialog
            .Values()
            .SelectMany(x => TaskMixin.ToSignal(() => notificationService.Show($"You entered \"{x}\" in the dialog ;)", "Howdy!")))
            .Subscribe();

        ShowDialog
            .Empties()
            .SelectMany(_ => TaskMixin.ToSignal(() => notificationService.Show("You dismissed the dialog", "Ouch!")))
            .Subscribe();
    }

    public ReactiveCommand<Unit, Maybe<string>> ShowDialog { get; set; }
}