using System.Diagnostics;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.Avalonia.Dialogs;

public class AdornerDialog : IDialog, ICloseable
{
    private readonly AdornerLayer layer;
    private readonly Stack<Control> dialogs = new();
    private TaskCompletionSource<bool>? currentDialog;

    public AdornerDialog(Visual control, DataTemplates? dataTemplates = null)
    {
        layer = AdornerLayer.GetAdornerLayer(control) 
            ?? throw new InvalidOperationException($"Could not get Adorner Layer from {control}");
        DataTemplates = dataTemplates.AsMaybe();
    }

    public Maybe<DataTemplates> DataTemplates { get; }

    public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
    {
        if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

        currentDialog = new TaskCompletionSource<bool>();
        var options = optionsFactory(this);

        var content = new DialogView
        {
            Content = viewModel,
            Options = options
        };

        content.DataTemplates.AddRange(GetDialogTemplates());

        var dialog = new DialogViewContainer
        {
            Title = title,
            Classes = { "Mobile" },
            Content = content,
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

    private DataTemplates GetDialogTemplates()
    {
        var map = Application.Current.AsMaybe().Map(Dialog.GetTemplates);
        var templates = DataTemplates.Or(map);
        return templates.GetValueOrDefault(new DataTemplates());
    }

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
}