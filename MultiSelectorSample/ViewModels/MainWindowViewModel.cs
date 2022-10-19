using System.Collections.Generic;

namespace MultiSelectorSample.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        Items = new List<ViewModel>()
        {
            new ViewModel { IsSelected = true },
            new ViewModel(),
            new ViewModel(),
            new ViewModel(),
        };
    }

    public List<ViewModel> Items { get; }
}