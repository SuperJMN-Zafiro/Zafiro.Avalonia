namespace Zafiro.Avalonia.Controls.Panels;

public class CardPanel : Panel
{
    // Public aspect-ratio property: width / height
    public static readonly StyledProperty<double> ItemAspectRatioProperty =
        AvaloniaProperty.Register<CardPanel, double>(
            nameof(ItemAspectRatio),
            defaultValue: 1.0
        );

    private Size _cellSize;

    // Estado privado para ArrangeOverride
    private int _columns;
    private int _rows;

    public double ItemAspectRatio
    {
        get => GetValue(ItemAspectRatioProperty);
        set => SetValue(ItemAspectRatioProperty, value);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        int count = Children.Count;
        if (count == 0)
            return new Size();

        // 1. Medir el máximo deseado de ancho/alto
        double maxDesiredWidth = 0;
        double maxDesiredHeight = 0;
        foreach (var child in Children)
        {
            child.Measure(Size.Infinity);
            var d = child.DesiredSize;
            if (d.Width > maxDesiredWidth) maxDesiredWidth = d.Width;
            if (d.Height > maxDesiredHeight) maxDesiredHeight = d.Height;
        }

        bool infiniteHeight = double.IsInfinity(availableSize.Height);

        if (infiniteHeight)
        {
            // Caso altura infinita: maximizar columnas posibles
            int bestCols = 1;
            for (int cols = 1; cols <= count; cols++)
            {
                double cellW = availableSize.Width / cols;
                double cellH = cellW / ItemAspectRatio;
                if (cellW < maxDesiredWidth || cellH < maxDesiredHeight)
                    break;
                bestCols = cols;
            }

            _columns = bestCols;
            _rows = (count + _columns - 1) / _columns;
            double w = availableSize.Width / _columns;
            _cellSize = new Size(w, w / ItemAspectRatio);
        }
        else
        {
            // Caso altura acotada: buscamos la cuadrícula que maximice el área de celda
            double bestArea = 0;
            for (int cols = 1; cols <= count; cols++)
            {
                int rows = (count + cols - 1) / cols;
                double cellW = availableSize.Width / cols;
                double cellH = availableSize.Height / rows;

                // Ajuste a la proporción
                double hBased = cellW / ItemAspectRatio;
                double wBased = cellH * ItemAspectRatio;
                double candW, candH;
                if (hBased <= cellH)
                {
                    candW = cellW;
                    candH = hBased;
                }
                else
                {
                    candW = wBased;
                    candH = cellH;
                }

                if (candW < maxDesiredWidth || candH < maxDesiredHeight)
                    continue;

                double area = candW * candH;
                if (area > bestArea)
                {
                    bestArea = area;
                    _columns = cols;
                    _rows = rows;
                    _cellSize = new Size(candW, candH);
                }
            }

            // Si ningún candidato cumple, hacemos fallback similar al infinito
            if (_columns == 0)
            {
                _columns = 1;
                _rows = count;
                double w = Math.Max(maxDesiredWidth, availableSize.Width);
                double h = w / ItemAspectRatio;
                if (h < maxDesiredHeight)
                {
                    h = maxDesiredHeight;
                    w = h * ItemAspectRatio;
                }

                _cellSize = new Size(w, h);
            }
        }

        return new Size(
            _cellSize.Width * _columns,
            _cellSize.Height * _rows
        );
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        for (int i = 0; i < Children.Count; i++)
        {
            int row = i / _columns;
            int col = i % _columns;
            var rect = new Rect(
                col * _cellSize.Width,
                row * _cellSize.Height,
                _cellSize.Width,
                _cellSize.Height
            );
            Children[i].Arrange(rect);
        }

        return new Size(
            _cellSize.Width * _columns,
            _cellSize.Height * _rows
        );
    }
}