using System.Windows.Input;
using ReactiveUI;
using TestApp.Samples.Adorners;
using Zafiro.UI.Navigation;

namespace TestApp.Samples.ControlsNew.Navigation;

[Icon("mdi-chevron-right")]
public class NavigationSampleViewModel(INavigator navigator) : ReactiveObject
{
    public ICommand Navigate => ReactiveCommand.CreateFromTask(() => navigator.Go<TargetViewModel>());
}