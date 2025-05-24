using ReactiveUI;
using ReactiveUI.SourceGenerators;
using Zafiro.UI.Shell.Utils;

namespace TestApp.Samples.Loading;

[Section("mdi-timer-sand-complete")]
public partial class LoadingSampleViewModel : ReactiveObject
{
    [Reactive] private bool isLoading;
}