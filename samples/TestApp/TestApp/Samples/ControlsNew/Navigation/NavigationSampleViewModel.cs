using System.Threading.Tasks;
using System.Windows.Input;
using ReactiveUI;
using Zafiro.UI.Navigation;

namespace TestApp.Samples.ControlsNew.Navigation;

public class NavigationSampleViewModel(INavigator navigator) : ReactiveObject
{
    public ICommand Navigate => ReactiveCommand.CreateFromTask(() => navigator.Go<TargetViewModel, string>("salute"));
}