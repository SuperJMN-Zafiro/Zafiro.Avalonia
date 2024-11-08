namespace Zafiro.Avalonia.Controls;

public enum ContentAlignment
{
    Start,
    Center,
    End
}

public class AspectRatioDecorator : Decorator
{
    public static readonly StyledProperty<ContentAlignment> ContentAlignmentProperty =
        AvaloniaProperty.Register<AspectRatioDecorator, ContentAlignment>(
            nameof(ContentAlignment), ContentAlignment.Center);

    public ContentAlignment ContentAlignment
    {
        get => GetValue(ContentAlignmentProperty);
        set => SetValue(ContentAlignmentProperty, value);
    }

    public static readonly StyledProperty<double> AspectRatioProperty =
        AvaloniaProperty.Register<AspectRatioDecorator, double>(
            nameof(AspectRatio), 1.0);

    public double AspectRatio
    {
        get => GetValue(AspectRatioProperty);
        set => SetValue(AspectRatioProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Child != null)
        {
            double desiredWidth = double.PositiveInfinity;
            double desiredHeight = double.PositiveInfinity;

            // Caso cuando ambos anchos y altos disponibles son finitos
            if (!double.IsInfinity(availableSize.Width) && !double.IsInfinity(availableSize.Height))
            {
                double availableAspectRatio = availableSize.Width / availableSize.Height;

                if (availableAspectRatio > AspectRatio)
                {
                    // Espacio sobrante horizontalmente
                    desiredHeight = availableSize.Height;
                    desiredWidth = desiredHeight * AspectRatio;
                }
                else
                {
                    // Espacio sobrante verticalmente
                    desiredWidth = availableSize.Width;
                    desiredHeight = desiredWidth / AspectRatio;
                }
            }
            else if (!double.IsInfinity(availableSize.Width))
            {
                // Ancho limitado, alto infinito
                desiredWidth = availableSize.Width;
                desiredHeight = desiredWidth / AspectRatio;
            }
            else if (!double.IsInfinity(availableSize.Height))
            {
                // Alto limitado, ancho infinito
                desiredHeight = availableSize.Height;
                desiredWidth = desiredHeight * AspectRatio;
            }
            else
            {
                // Ambos anchos y altos son infinitos
                // Podemos definir un tamaño por defecto o usar el tamaño deseado del hijo
                desiredWidth = 100; // Tamaño por defecto
                desiredHeight = desiredWidth / AspectRatio;
            }

            // Medir el hijo con el tamaño calculado
            Child.Measure(new Size(desiredWidth, desiredHeight));

            return new Size(desiredWidth, desiredHeight);
        }

        return new Size();
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Child != null)
        {
            double width;
            double height;
            double offsetX = 0;
            double offsetY = 0;

            double finalAspectRatio = finalSize.Width / finalSize.Height;

            if (finalAspectRatio > AspectRatio)
            {
                // Espacio sobrante horizontalmente
                height = finalSize.Height;
                width = height * AspectRatio;
                double remainingWidth = finalSize.Width - width;

                // Aplicar alineación horizontal
                switch (ContentAlignment)
                {
                    case ContentAlignment.Start:
                        offsetX = 0;
                        break;
                    case ContentAlignment.Center:
                        offsetX = remainingWidth / 2;
                        break;
                    case ContentAlignment.End:
                        offsetX = remainingWidth;
                        break;
                }

                offsetY = 0;
            }
            else
            {
                // Espacio sobrante verticalmente
                width = finalSize.Width;
                height = width / AspectRatio;
                double remainingHeight = finalSize.Height - height;

                // Aplicar alineación vertical
                switch (ContentAlignment)
                {
                    case ContentAlignment.Start:
                        offsetY = 0;
                        break;
                    case ContentAlignment.Center:
                        offsetY = remainingHeight / 2;
                        break;
                    case ContentAlignment.End:
                        offsetY = remainingHeight;
                        break;
                }

                offsetX = 0;
            }

            Child.Arrange(new Rect(offsetX, offsetY, width, height));
            return finalSize;
        }

        return finalSize;
    }
}
