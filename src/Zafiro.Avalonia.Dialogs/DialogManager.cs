using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using CSharpFunctionalExtensions;
using Zafiro.Avalonia.Dialogs.Views;
using Zafiro.Avalonia.Mixins;

namespace Zafiro.Avalonia.Dialogs
{
    public class StackedDialog : IDialog
    {
        private static Window? dialogWindow;
        private static readonly Stack<DialogContext> DialogStack = new();
        private static Result<Window> MainWindow => Application.Current!.TopLevel().Map(level => level as Window).EnsureNotNull("TopLevel is not a Window!");

        private class DialogContext
        {
            public object ViewModel { get; }
            public string Title { get; }
            public IEnumerable<IOption> Options { get; }
            public TaskCompletionSource<bool> CompletionSource { get; }

            public DialogContext(object viewModel, string title, IEnumerable<IOption> options, TaskCompletionSource<bool> completionSource)
            {
                ViewModel = viewModel;
                Title = title;
                Options = options;
                CompletionSource = completionSource;
            }
        }

        private class DialogCloseable : ICloseable
        {
            private readonly TaskCompletionSource<bool> _completionSource;
            private readonly bool _result;

            public DialogCloseable(TaskCompletionSource<bool> completionSource, bool result)
            {
                _completionSource = completionSource;
                _result = result;
            }

            public void Close()
            {
                // Completa el diálogo actual con el resultado correspondiente
                _completionSource.TrySetResult(_result);
                
                // Quita el diálogo actual de la pila
                if (DialogStack.Count > 0)
                {
                    DialogStack.Pop();
                }

                // Si hay más diálogos en la pila, muestra el siguiente
                if (DialogStack.Count > 0)
                {
                    var nextDialog = DialogStack.Peek();
                    UpdateDialogContent(nextDialog);
                }
                else
                {
                    // Si no hay más diálogos, cierra la ventana
                    dialogWindow?.Close();
                    dialogWindow = null;
                }
            }

            public void Dismiss()
            {
                // Completa el diálogo actual con resultado falso (cancelado/descartado)
                _completionSource.TrySetResult(false);
                
                // Quita el diálogo actual de la pila
                if (DialogStack.Count > 0)
                {
                    DialogStack.Pop();
                }

                // Si hay más diálogos en la pila, muestra el siguiente
                if (DialogStack.Count > 0)
                {
                    var nextDialog = DialogStack.Peek();
                    UpdateDialogContent(nextDialog);
                }
                else
                {
                    // Si no hay más diálogos, cierra la ventana
                    dialogWindow?.Close();
                    dialogWindow = null;
                }
            }
        }

        private static void UpdateDialogContent(DialogContext dialogContext)
        {
            if (dialogWindow != null)
            {
                dialogWindow.Title = dialogContext.Title;
                dialogWindow.Content = new DialogControl
                {
                    Title = Maybe<string>.None,
                    Content = dialogContext.ViewModel,
                    Options = dialogContext.Options
                };
            }
        }

        public async Task<bool> Show(object viewModel, string title, Func<ICloseable, IEnumerable<IOption>> optionsFactory)
        {
            if (viewModel == null) throw new ArgumentNullException(nameof(viewModel));

            var completionSource = new TaskCompletionSource<bool>();
            var closeable = new DialogCloseable(completionSource, true);
            var options = optionsFactory(closeable).ToList();
            
            // Crea una instancia de contexto para el diálogo actual
            var dialogContext = new DialogContext(viewModel, title, options, completionSource);
            
            // Añade el diálogo a la pila
            DialogStack.Push(dialogContext);

            // Si no hay ventana de diálogo, crea una nueva
            if (dialogWindow == null)
            {
                dialogWindow = new Window
                {
                    WindowStartupLocation = WindowStartupLocation.CenterOwner,
                    CanResize = false,
                    Icon = MainWindow.Value.Icon,
                    SizeToContent = SizeToContent.WidthAndHeight,
                    MaxWidth = 800,
                    MaxHeight = 800,
                    MinWidth = 300,
                    MinHeight = 200
                };

                if (Debugger.IsAttached)
                {
                    dialogWindow.AttachDevTools();
                }

                // Maneja el evento de cierre de la ventana para completar todos los diálogos pendientes
                dialogWindow.Closed += (sender, args) =>
                {
                    while (DialogStack.Count > 0)
                    {
                        var dialog = DialogStack.Pop();
                        dialog.CompletionSource.TrySetResult(false);
                    }
                    dialogWindow = null;
                };

                // Actualiza el contenido con el diálogo actual
                UpdateDialogContent(dialogContext);

                // Muestra la ventana de diálogo
                dialogWindow.Show(MainWindow.Value);
            }
            else
            {
                // Si ya hay una ventana de diálogo, actualiza su contenido
                UpdateDialogContent(dialogContext);
            }

            // Espera a que se complete el diálogo actual
            return await completionSource.Task;
        }
    }
}