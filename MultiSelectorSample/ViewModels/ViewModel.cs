using MultiSelectorSample.Views;
using ReactiveUI.Fody.Helpers;

namespace MultiSelectorSample.ViewModels;

public class ViewModel : ViewModelBase, ISelectable
{
    [Reactive]
    public bool IsSelected { get; set; }
}