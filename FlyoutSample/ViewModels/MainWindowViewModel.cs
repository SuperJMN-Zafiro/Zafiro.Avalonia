using System.Reactive;
using Avalonia.Controls.ApplicationLifetimes;
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
                FocusManager.Instance.Focus(((ClassicDesktopStyleApplicationLifetime)App.Current.ApplicationLifetime).MainWindow);
                IsFlyoutOpen = false;
            });
        }

        public ReactiveCommand<Unit, Unit> HideFlyout { get; }

        [Reactive]
        public bool IsFlyoutOpen { get; set; }
    }
}
