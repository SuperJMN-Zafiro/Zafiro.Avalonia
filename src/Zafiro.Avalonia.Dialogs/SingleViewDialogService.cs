using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Layout;
using CSharpFunctionalExtensions;
using ReactiveUI;

namespace Zafiro.Avalonia.Dialogs;

// public class SingleViewDialogService : IDialogService
// {
//     private readonly Stack<Control> dialogs = new();
//     private readonly AdornerLayer layer;
//     private readonly Subject<Unit> closeSubject = new();
//
//     public SingleViewDialogService(Visual control)
//     {
//         layer = AdornerLayer.GetAdornerLayer(control) ?? throw new InvalidOperationException($"Could not get Adorner Layer from {control}");
//     }
//
//     public async Task<Maybe<TResult>> ShowDialog<TViewModel, TResult>(TViewModel viewModel, string title, Func<TViewModel, IObservable<TResult>> results, Maybe<Action<ConfigureSizeContext>> configureWindowActionOverride, params OptionConfiguration<TViewModel, TResult>[] options) where TViewModel : UI.IResult<TResult>
//     {
//         if (viewModel == null)
//         {
//             throw new ArgumentNullException(nameof(viewModel));
//         }
//
//         var view = new DialogView
//         {
//             DataContext = new DialogViewModel(viewModel, options.Select(x => new Option(x.Title, command: x.Factory(viewModel))).ToArray())
//         };
//
//         var dialog = new DialogViewContainer
//         {
//             Title = title,
//             Classes = { "Mobile" },
//             Close = ReactiveCommand.Create(() =>
//             {
//                 Close();
//                 closeSubject.OnNext(Unit.Default);
//             }),
//             VerticalAlignment = VerticalAlignment.Stretch,
//             HorizontalAlignment = HorizontalAlignment.Stretch,
//             CornerRadius = new CornerRadius(10),
//             Content = view,
//             [!Layoutable.HeightProperty] = layer.Parent!.GetObservable(Visual.BoundsProperty).Select(rect => rect.Height).ToBinding(),
//             [!Layoutable.WidthProperty] = layer.Parent!.GetObservable(Visual.BoundsProperty).Select(rect => rect.Width).ToBinding()
//         };
//
//         layer.Children.Add(dialog);
//         dialogs.Push(dialog);
//
//         var firstResult = results(viewModel)
//             .Select(x => Maybe.From(x))
//             .ObserveOn(SynchronizationContext.Current!)
//             .Do(_ => Close());
//
//         var closeResult = closeSubject.Select(_ => Maybe<TResult>.None);
//         var merged = firstResult.Merge(closeResult).FirstAsync();
//         var result = await merged;
//         
//         return result;
//     }
//
//     private void Close()
//     {
//         var valueTuple = dialogs.Pop();
//         layer.Children.Remove(valueTuple);
//     }
// }