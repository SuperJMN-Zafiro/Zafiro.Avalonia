using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.MigrateToZafiro;

namespace Zafiro.Avalonia.Dialogs;

public class SingleViewDialogService : IDialogService
{
    private readonly Stack<(Control, TaskCompletionSource)> dialogs = new();
    private readonly AdornerLayer layer;

    public SingleViewDialogService(Visual control)
    {
        layer = AdornerLayer.GetAdornerLayer(control) ?? throw new InvalidOperationException($"Could not get Adorner Layer from {control}");
    }

    public async Task<Maybe<TResult>> ShowDialog<TViewModel, TResult>(TViewModel viewModel, string title, Func<TViewModel, IObservable<TResult>> results, params OptionConfiguration<TViewModel, TResult>[] options) where TViewModel : UI.IResult<TResult>
    {
        if (viewModel == null)
        {
            throw new ArgumentNullException(nameof(viewModel));
        }

        var tcs = new TaskCompletionSource();

        var view = new DialogView
        {
            DataContext = new DialogViewModel(viewModel, title, options.Select(x => new Option(x.Title, command: x.Factory(viewModel))).ToArray())
        };

        var dialog = new DialogViewContainer
        {
            Title = title,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
            CornerRadius = new CornerRadius(10),
            Content = view,
            [!Layoutable.HeightProperty] = layer.Parent!.GetObservable(Visual.BoundsProperty).Select(rect => rect.Height).ToBinding(),
            [!Layoutable.WidthProperty] = layer.Parent!.GetObservable(Visual.BoundsProperty).Select(rect => rect.Width).ToBinding()
        };

        layer.Children.Add(dialog);
        dialogs.Push((dialog, tcs));

        var result = await results(viewModel).Take(1);
        Close();
        return result;
    }

    private void Close()
    {
        var valueTuple = dialogs.Pop();
        layer.Children.Remove(valueTuple.Item1);
        valueTuple.Item2.SetResult();
    }
}