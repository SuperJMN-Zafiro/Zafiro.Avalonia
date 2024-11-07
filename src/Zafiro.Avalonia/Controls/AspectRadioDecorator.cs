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

    public double AspectRatio { get; set; } = 1.0; // Ancho / Alto

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Child != null)
        {
            double width = availableSize.Width;
            double height = width / AspectRatio;

            if (height > availableSize.Height)
            {
                height = availableSize.Height;
                width = height * AspectRatio;
            }

            Child.Measure(new Size(width, height));
            return new Size(width, height);
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
                // Hay espacio sobrante horizontalmente
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
                // Hay espacio sobrante verticalmente
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
        }

        return finalSize;
    }
}