using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Views;

namespace Zafiro.Avalonia.Dialogs;

public class AdornerDialog(Visual control) : IDialog, ICloseable
{
    private readonly Stack<Control> dialogs = new();

    private readonly AdornerLayer layer = AdornerLayer.GetAdornerLayer(control)
                                          ?? throw new InvalidOperationException($"Could not get Adorner Layer from {control}");

    private TaskCompletionSource<bool>? currentDialog;

    public void Close()
    {
        if (dialogs.Count > 0)
        {
            var dialog = dialogs.Pop();
            layer.Children.Remove(dialog);
        }

        currentDialog?.TrySetResult(true);
        currentDialog = null;
    }

    public void Dismiss()
    {
        if (dialogs.Count > 0)
        {
            var dialog = dialogs.Pop();
            layer.Children.Remove(dialog);
        }

        currentDialog?.TrySetResult(false);
        currentDialog = null;
    }


    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        currentDialog = new TaskCompletionSource<bool>();
        var options = optionsFactory(this);


        var dialog = new DialogViewContainer
        {
            Title = title,
            Content = new DialogControl()
            {
                Content = viewModel,
                Options = options,
                Title = Maybe<string>.None,
            },
            Close = ReactiveCommand.Create(() => Dismiss()),
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(10)
        };

        dialog[!Layoutable.HeightProperty] = layer.Parent!
            .GetObservable(Visual.BoundsProperty)
            .Select(rect => rect.Height)
            .ToBinding();

        dialog[!Layoutable.WidthProperty] = layer.Parent!
            .GetObservable(Visual.BoundsProperty)
            .Select(rect => rect.Width)
            .ToBinding();

        layer.Children.Add(dialog);
        dialogs.Push(dialog);

        var result = await currentDialog.Task.ConfigureAwait(false);
        return result;
    }
}