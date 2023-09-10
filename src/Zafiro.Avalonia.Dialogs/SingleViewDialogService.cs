using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;

namespace Zafiro.Avalonia.Dialogs;

public class SingleViewDialogService : DialogService, ICloseable
{
    private readonly Stack<(Control, TaskCompletionSource)> dialogs = new();
    private readonly AdornerLayer layer;

    public SingleViewDialogService(Visual control)
    {
        layer = AdornerLayer.GetAdornerLayer(control) ?? throw new InvalidOperationException($"Could not get Adorner Layer from {control}");
    }

    public override Task ShowDialog<T>(T viewModel, string title, params OptionConfiguration<T>[] options)
    {
        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        var tcs = new TaskCompletionSource();

        var view = new DialogView
        {
            DataContext = new DialogViewModel(viewModel, title, CreateOptions(viewModel, this, options).ToArray()),
        };

        var dialog = new DialogViewContainer()
        {
            Title = title,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(10),
            Content = view,
            [!Layoutable.HeightProperty] = layer.Parent!.GetObservable(Visual.BoundsProperty).Select(rect => rect.Height).ToBinding(),
            [!Layoutable.WidthProperty] = layer.Parent!.GetObservable(Visual.BoundsProperty).Select(rect => rect.Width).ToBinding(),
        };

        layer.Children.Add(dialog);
        dialogs.Push((dialog, tcs));

        return tcs.Task;
    }

    public void Close()
    {
        var valueTuple = dialogs.Pop();
        layer.Children.Remove(valueTuple.Item1);
        valueTuple.Item2.SetResult();
    }
}