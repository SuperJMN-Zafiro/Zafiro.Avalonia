using System.Windows.Input;
using ReactiveUI;
using Zafiro.UI.Navigation;
using Zafiro.UI.Shell.Utils;

namespace TestApp.Samples.Navigation;

[Section(icon: "mdi-chevron-right", sortIndex: 13)]
public class NavigationSampleViewModel(INavigator navigator) : ReactiveObject
{
    public ICommand Navigate => ReactiveCommand.CreateFromTask(() => navigator.Go<TargetViewModel>());
}