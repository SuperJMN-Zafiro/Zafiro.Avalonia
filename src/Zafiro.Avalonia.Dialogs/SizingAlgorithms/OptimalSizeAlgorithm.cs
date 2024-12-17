using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs.SizingAlgorithms;

public class OptimalSizeAlgorithm : IChildSizingAlgorithm
{
    public static OptimalSizeAlgorithm Instance { get; } = new OptimalSizeAlgorithm();

    private const double MinimumWidth = 400;
    private const double MinimumHeight = 300;
    
    private const double MaximumScreenPercentage = 0.95;
    private const double PreferredParentWindowPercentage = 0.95;

    // Aumentamos el factor de escala para dar más holgura
    private const double ContentScaleFactor = 1.5;
    
    private const double ContentPadding = 40;

    public Size GetWindowSize(
        Control content,
        Size? screenSize = null,
        Size? parentWindowSize = null)
    {
        var maxWidth = double.PositiveInfinity;
        var maxHeight = double.PositiveInfinity;

        if (screenSize.HasValue)
        {
            maxWidth = screenSize.Value.Width * MaximumScreenPercentage;
            maxHeight = screenSize.Value.Height * MaximumScreenPercentage;
        }

        if (parentWindowSize.HasValue)
        {
            maxWidth = Math.Min(maxWidth, parentWindowSize.Value.Width * PreferredParentWindowPercentage);
            maxHeight = Math.Min(maxHeight, parentWindowSize.Value.Height * PreferredParentWindowPercentage);
        }

        // Medimos el contenido con los máximos calculados
        content.Measure(new Size(maxWidth, maxHeight));
        var desiredSize = content.DesiredSize;

        // Escalamos el contenido y añadimos padding
        var finalWidth = Math.Max(desiredSize.Width * ContentScaleFactor + ContentPadding * 2, MinimumWidth);
        var finalHeight = Math.Max(desiredSize.Height * ContentScaleFactor + ContentPadding * 2, MinimumHeight);

        // Añadimos un mínimo relativo a la ventana padre, si existe
        if (parentWindowSize.HasValue)
        {
            // Por ejemplo: no sea más pequeño que la mitad de la ventana padre
            finalWidth = Math.Max(finalWidth, parentWindowSize.Value.Width * 0.5);
            finalHeight = Math.Max(finalHeight, parentWindowSize.Value.Height * 0.5);
        }

        // Ajustamos contra los máximos globales
        finalWidth = Math.Min(finalWidth, maxWidth);
        finalHeight = Math.Min(finalHeight, maxHeight);

        return new Size(Math.Round(finalWidth), Math.Round(finalHeight));
    }
}