using System;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia.Controls.Primitives;
using Zafiro.UI.Commands;
using Zafiro.UI.Wizards.Slim;

namespace Zafiro.Avalonia.Controls.Wizards.Slim;

public class WizardNavigator : TemplatedControl
{
    public static readonly StyledProperty<ISlimWizard> WizardProperty = AvaloniaProperty.Register<WizardNavigator, ISlimWizard>(
        nameof(Wizard));

    public static readonly StyledProperty<IEnhancedCommand> CancelProperty = AvaloniaProperty.Register<WizardNavigator, IEnhancedCommand>(
        nameof(Cancel));

    public static readonly DirectProperty<WizardNavigator, bool> CanCancelProperty =
        AvaloniaProperty.RegisterDirect<WizardNavigator, bool>(nameof(CanCancel), o => o.CanCancel);

    private IDisposable? canCancelSubscription;
    private bool canCancel;

    public ISlimWizard Wizard
    {
        get => GetValue(WizardProperty);
        set => SetValue(WizardProperty, value);
    }

    public IEnhancedCommand Cancel
    {
        get => GetValue(CancelProperty);
        set => SetValue(CancelProperty, value);
    }

    public bool CanCancel
    {
        get => canCancel;
        private set => SetAndRaise(CanCancelProperty, ref canCancel, value);
    }

    static WizardNavigator()
    {
        WizardProperty.Changed.Subscribe(args =>
        {
            if (args.Sender is WizardNavigator navigator)
            {
                navigator.SetupCanCancel(args.NewValue.GetValueOrDefault<ISlimWizard>());
            }
        });
    }

    private void SetupCanCancel(ISlimWizard wizard)
    {
        canCancelSubscription?.Dispose();
        if (wizard is null)
        {
            CanCancel = false;
            return;
        }

        var currentPage = Observable.FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => ((INotifyPropertyChanged)wizard).PropertyChanged += h,
                h => ((INotifyPropertyChanged)wizard).PropertyChanged -= h)
            .Where(e => e.EventArgs.PropertyName == nameof(ISlimWizard.CurrentPage))
            .Select(_ => wizard.CurrentPage)
            .StartWith(wizard.CurrentPage);

        var backCanExecute = ((IReactiveCommand)wizard.Back).CanExecute
            .StartWith(((ICommand)wizard.Back).CanExecute(null));

        var nextExecuting = ((IReactiveCommand)wizard.Next).IsExecuting.StartWith(false);

        var onCompletion = currentPage.CombineLatest(backCanExecute, (page, canBack) =>
            page.Index == wizard.TotalPages - 1 && !canBack);

        canCancelSubscription = onCompletion
            .CombineLatest(nextExecuting, (completion, executing) => !completion && !executing)
            .Subscribe(value => CanCancel = value);
    }
}
