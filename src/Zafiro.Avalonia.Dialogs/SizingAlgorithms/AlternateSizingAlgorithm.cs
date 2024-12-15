using Avalonia;
using Avalonia.Controls;

namespace Zafiro.Avalonia.Dialogs.Simple;

public class AlternateSizingAlgorithm : IChildSizingAlgorithm
{
    public static AlternateSizingAlgorithm Instance { get; } = new AlternateSizingAlgorithm();

    // Tamaño mínimo absoluto (nunca más pequeño que esto)
    private const double AbsoluteMinimumWidth = 400;
    private const double AbsoluteMinimumHeight = 300;

    // Tamaño proporcional al tamaño de la ventana padre
    private const double MinimumParentWindowPercentage = 0.3;

    // Porcentajes máximos
    private const double MaximumScreenPercentage = 0.95;
    private const double PreferredParentWindowPercentage = 0.85;

    // Escala de contenido y margen
    private const double ContentScaleFactor = 1.3;
    private const double ContentPadding = 40;

    public Size GetWindowSize(
        Control content,
        Size? screenSize = null,
        Size? parentWindowSize = null)
    {
        double maxWidth = double.PositiveInfinity;
        double maxHeight = double.PositiveInfinity;

        // Limitar al tamaño de la pantalla
        if (screenSize.HasValue)
        {
            maxWidth = screenSize.Value.Width * MaximumScreenPercentage;
            maxHeight = screenSize.Value.Height * MaximumScreenPercentage;
        }

        // Limitar al tamaño de la ventana padre
        if (parentWindowSize.HasValue)
        {
            maxWidth = Math.Min(maxWidth, parentWindowSize.Value.Width * PreferredParentWindowPercentage);
            maxHeight = Math.Min(maxHeight, parentWindowSize.Value.Height * PreferredParentWindowPercentage);
        }

        // Calcular tamaños mínimos dinámicos basados en el tamaño de la ventana padre
        double minimumWidth = AbsoluteMinimumWidth;
        double minimumHeight = AbsoluteMinimumHeight;
        if (parentWindowSize.HasValue)
        {
            minimumWidth = Math.Max(minimumWidth, parentWindowSize.Value.Width * MinimumParentWindowPercentage);
            minimumHeight = Math.Max(minimumHeight, parentWindowSize.Value.Height * MinimumParentWindowPercentage);
        }

        // Medir el contenido inicial
        content.Measure(new Size(maxWidth, maxHeight));
        var desiredSize = content.DesiredSize;

        // Escalar el tamaño base
        double finalWidth = Math.Max(desiredSize.Width * ContentScaleFactor + ContentPadding * 2, minimumWidth);
        double finalHeight = Math.Max(desiredSize.Height * ContentScaleFactor + ContentPadding * 2, minimumHeight);

        // Aplicar restricciones máximas
        finalWidth = Math.Min(finalWidth, maxWidth);
        finalHeight = Math.Min(finalHeight, maxHeight);

        return new Size(Math.Round(finalWidth), Math.Round(finalHeight));
    }
}
