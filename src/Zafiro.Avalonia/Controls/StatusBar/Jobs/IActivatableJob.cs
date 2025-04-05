using System.Windows.Input;
using Zafiro.UI.Jobs;

namespace Zafiro.Avalonia.Controls.StatusBar.Jobs;

public interface IActivatableJob : IJob
{
    ICommand? ActivateCommand { get; }
}