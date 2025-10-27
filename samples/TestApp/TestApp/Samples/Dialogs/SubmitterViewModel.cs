using CSharpFunctionalExtensions;
using ReactiveUI;
using Zafiro.UI.Commands;

namespace TestApp.Samples.Dialogs;

public class SubmitterViewModel
{
    public SubmitterViewModel()
    {
        Submit = ReactiveCommand.Create(() => Result.Success(1)).Enhance();
    }

    public IEnhancedCommand<Result<int>> Submit { get; }
}