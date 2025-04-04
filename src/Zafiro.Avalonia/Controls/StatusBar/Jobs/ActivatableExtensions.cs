using System.Windows.Input;
using Zafiro.UI.Jobs;

namespace Zafiro.Avalonia.Controls.StatusBar.Jobs;

public static class ActivatableExtensions
{
    public static IActivatableJob ToActivatableJob(this IJob job, ICommand activateCommand)
    {
        if (job == null)
        {
            throw new ArgumentNullException(nameof(job));
        }

        return new ActivatableJob(job, activateCommand);
    }
}