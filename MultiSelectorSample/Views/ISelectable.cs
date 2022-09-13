using System.ComponentModel;

namespace MultiSelectorSample.Views;

public interface ISelectable : INotifyPropertyChanged
{
    public bool IsSelected { get; set; }
}