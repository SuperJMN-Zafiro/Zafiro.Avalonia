using System.Collections.Generic;

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