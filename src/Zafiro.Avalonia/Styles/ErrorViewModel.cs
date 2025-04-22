using ReactiveUI.SourceGenerators;
using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace Zafiro.Avalonia.Styles;

public partial class ErrorViewModel : ReactiveValidationObject
{
    [Reactive] private string? first;

    [Reactive] private string? second;

    public ErrorViewModel()
    {
        this.ValidationRule(x => x.First, s => !string.IsNullOrWhiteSpace(s), "Error 1");
        this.ValidationRule(x => x.Second, s => !string.IsNullOrWhiteSpace(s), "Error 2");
    }
}