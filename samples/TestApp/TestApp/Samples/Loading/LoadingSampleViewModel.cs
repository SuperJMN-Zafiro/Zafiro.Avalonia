using ReactiveUI;
using ReactiveUI.SourceGenerators;
using TestApp.Samples.Adorners;

namespace TestApp.Samples.ControlsNew.Loading;

[Icon("mdi-timer-sand-complete")]
public partial class LoadingSampleViewModel : ReactiveObject
{
    [Reactive] private bool isLoading;
}