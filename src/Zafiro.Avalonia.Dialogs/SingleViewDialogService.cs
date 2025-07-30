using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs.Views;

namespace Zafiro.Avalonia.Dialogs;

public class AdornerDialog : IDialog, ICloseable
{
    private readonly Stack<Control> dialogs = new();
    
    private TaskCompletionSource<bool>? currentDialog;
    private readonly Lazy<AdornerLayer> adornerLayerLazy;

    public AdornerDialog(Func<AdornerLayer> getAdornerLayer)
    {
        adornerLayerLazy = new Lazy<AdornerLayer>(() => getAdornerLayer());
    }


    public void Close()
    {
        if (dialogs.Count > 0)
        {
            var dialog = dialogs.Pop();
            adornerLayerLazy.Value.Children.Remove(dialog);
        }

        currentDialog?.TrySetResult(true);
        currentDialog = null;
    }

    public void Dismiss()
    {
        if (dialogs.Count > 0)
        {
            var dialog = dialogs.Pop();
            adornerLayerLazy.Value.Children.Remove(dialog);
        }

        currentDialog?.TrySetResult(false);
        currentDialog = null;
    }
    
    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        currentDialog = new TaskCompletionSource<bool>();
        var options = optionsFactory(this);

        var dialogControl = new DialogControl
        {
            Content = viewModel,
            Options = options,
        };

        if (viewModel is IDialogHeaderProvider headerProvider)
        {
            dialogControl.Header = headerProvider.Header;
        }
        else
        {
            dialogControl.Title = title;
        }

        var dialog = new DialogViewContainer
        {
            Title = title,
            Content = dialogControl,
            Close = ReactiveCommand.Create(() => Dismiss()),
        };

        var adornerLayer = adornerLayerLazy.Value;
        
        dialog[!Layoutable.HeightProperty] = adornerLayer.Parent!
            .GetObservable(Visual.BoundsProperty)
            .Select(rect => rect.Height)
            .ToBinding();

        dialog[!Layoutable.WidthProperty] = adornerLayer.Parent!
            .GetObservable(Visual.BoundsProperty)
            .Select(rect => rect.Width)
            .ToBinding();

        adornerLayer.Children.Add(dialog);
        dialogs.Push(dialog);

        var result = await currentDialog.Task.ConfigureAwait(false);
        return result;
    }
}