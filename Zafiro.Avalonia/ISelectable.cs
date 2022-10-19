using System.ComponentModel;

namespace Zafiro.Avalonia;

public interface ISelectable : INotifyPropertyChanged
{
    public bool IsSelected { get; set; }
}