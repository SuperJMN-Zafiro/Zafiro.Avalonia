namespace Zafiro.Avalonia.Controls;

public class CenteredDistributorPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            // Medir cada hijo con el alto disponible y ancho infinito
            child.Measure(new Size(Double.PositiveInfinity, availableSize.Height));
        }
        // Retornar el tamaño total disponible
        return new Size(availableSize.Width, availableSize.Height);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        int count = Children.Count;
        if (count == 0)
            return finalSize;

        for (int i = 0; i < count; i++)
        {
            var child = Children[i];
            double childId = i;

            double xPosition;
            if (count == 1)
            {
                // Si solo hay un hijo, centrarlo en el medio del panel
                xPosition = finalSize.Width / 2;
            }
            else
            {
                // Distribuir los hijos entre 0 y finalSize.Width
                xPosition = (finalSize.Width / (count - 1)) * childId;
            }

            // Ajustar para alinear horizontalmente el hijo respecto al punto xPosition
            double childWidth = child.DesiredSize.Width;
            double x = xPosition - (childWidth / 2);

            // No limitar la posición x para permitir que los hijos se extiendan más allá del panel si es necesario
            child.Arrange(new Rect(new Point(x, 0), new Size(childWidth, finalSize.Height)));
        }

        return finalSize;
    }
}