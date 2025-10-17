using Avalonia;

namespace Zafiro.Avalonia.Dialogs;

/// <summary>
/// Strategy for calculating dialog sizes based on content and available space.
/// </summary>
public interface IDialogSizingStrategy
{
    /// <summary>
    /// Calculates the desired size for a dialog.
    /// </summary>
    /// <param name="content">The dialog content (view/viewmodel).</param>
    /// <param name="availableSize">Available screen space.</param>
    /// <param name="config">Sizing configuration.</param>
    /// <returns>Calculated size for the dialog.</returns>
    Size Calculate(object content, Size availableSize, AdaptiveDialogSizer.SizingConfig config);
}