using ReactiveUI.Validation.Extensions;
using ReactiveUI.Validation.Helpers;

namespace Zafiro.Avalonia.Controls.StringEditor;

public static class ValidationMixin
{
    public static ValidationHelper ValidationRule(this EditableString wrapper, Func<string?, bool> isValid, string message)
    {
        return wrapper.WorkInstanceOfType.ValidationRule(model => model.Text, isValid, message);
    }
}