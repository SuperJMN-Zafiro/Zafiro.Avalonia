using System.Collections.Generic;
using MultiSelectorSample.Views;
using ReactiveUI.Fody.Helpers;

namespace MultiSelectorSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public MainWindowViewModel()
    {
        Items = new List<ViewModel>
        {
            new ViewModel(),
            new ViewModel(),
            new ViewModel(),
            new ViewModel(),
        };
    }

    public List<ViewModel> Items { get; }
}


public class ViewModel : ViewModelBase, ISelectableNotify
{
    [Reactive]
    public bool IsSelected { get; set; }
}