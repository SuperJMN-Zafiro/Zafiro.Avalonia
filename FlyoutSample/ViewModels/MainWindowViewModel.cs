using System.Reactive;
using Avalonia.Input;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace FlyoutSample.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        public MainWindowViewModel()
        {
            HideFlyout = ReactiveCommand.Create(() =>
            {
                IsFlyoutOpen = false;
                FocusManager.Instance.Focus(null);
            });
        }

        public ReactiveCommand<Unit, Unit> HideFlyout { get; }

        [Reactive]
        public bool IsFlyoutOpen { get; set; }
    }
}
