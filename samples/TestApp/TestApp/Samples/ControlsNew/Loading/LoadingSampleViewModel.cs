using ReactiveUI;
using ReactiveUI.SourceGenerators;

namespace TestApp.Samples.ControlsNew.Loading;

public partial class LoadingSampleViewModel : ReactiveObject
{
    [Reactive] private bool isLoading;
}