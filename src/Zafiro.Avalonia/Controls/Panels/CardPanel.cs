namespace Zafiro.Avalonia.Controls.Panels
{
    public class CardPanel : Panel
    {
        // Public aspect-ratio property: width / height
        public static readonly StyledProperty<double> ItemAspectRatioProperty =
            AvaloniaProperty.Register<CardPanel, double>(
                nameof(ItemAspectRatio),
                defaultValue: 1.0
            );

        // Public maximum width per item; defaults to infinite
        public static readonly StyledProperty<double> MaxItemWidthProperty =
            AvaloniaProperty.Register<CardPanel, double>(
                nameof(MaxItemWidth),
                double.PositiveInfinity
            );

        private Size _cellSize;
        private int _columns;
        private int _rows;

        public double ItemAspectRatio
        {
            get => GetValue(ItemAspectRatioProperty);
            set => SetValue(ItemAspectRatioProperty, value);
        }

        public double MaxItemWidth
        {
            get => GetValue(MaxItemWidthProperty);
            set => SetValue(MaxItemWidthProperty, value);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var count = Children.Count;
            if (count == 0)
                return new Size();

            // 1. Determine max desired size among children
            double maxDesiredWidth = 0;
            double maxDesiredHeight = 0;
            foreach (var child in Children)
            {
                child.Measure(Size.Infinity);
                var d = child.DesiredSize;
                if (d.Width > maxDesiredWidth) maxDesiredWidth = d.Width;
                if (d.Height > maxDesiredHeight) maxDesiredHeight = d.Height;
            }

            var infiniteHeight = double.IsInfinity(availableSize.Height);

            if (infiniteHeight)
            {
                // Case: infinite height, maximize number of columns
                int bestCols = 1;
                for (var cols = 1; cols <= count; cols++)
                {
                    var rawW = availableSize.Width / cols;
                    var rawH = rawW / ItemAspectRatio;
                    if (rawW < maxDesiredWidth || rawH < maxDesiredHeight)
                        break;
                    bestCols = cols;
                }

                _columns = bestCols;
                _rows = (count + _columns - 1) / _columns;

                var rawCellW = availableSize.Width / _columns;
                var cellW = Math.Min(rawCellW, MaxItemWidth);
                var cellH = cellW / ItemAspectRatio;
                _cellSize = new Size(cellW, cellH);
            }
            else
            {
                // Case: bounded height, pick grid maximizing cell area
                double bestArea = 0;
                for (var cols = 1; cols <= count; cols++)
                {
                    var rows = (count + cols - 1) / cols;
                    var availableW = availableSize.Width / cols;
                    var availableH = availableSize.Height / rows;

                    // Fit to aspect ratio
                    var hBased = availableW / ItemAspectRatio;
                    var wBased = availableH * ItemAspectRatio;
                    double candW, candH;
                    if (hBased <= availableH)
                    {
                        candW = availableW;
                        candH = hBased;
                    }
                    else
                    {
                        candW = wBased;
                        candH = availableH;
                    }

                    // Apply maximum width constraint
                    candW = Math.Min(candW, MaxItemWidth);
                    candH = candW / ItemAspectRatio;

                    if (candW < maxDesiredWidth || candH < maxDesiredHeight)
                        continue;

                    var area = candW * candH;
                    if (area > bestArea)
                    {
                        bestArea = area;
                        _columns = cols;
                        _rows = rows;
                        _cellSize = new Size(candW, candH);
                    }
                }

                // Fallback if no candidate fits
                if (_columns == 0)
                {
                    _columns = 1;
                    _rows = count;
                    var w = Math.Max(maxDesiredWidth, availableSize.Width);
                    w = Math.Min(w, MaxItemWidth);
                    var h = w / ItemAspectRatio;
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
            for (var i = 0; i < Children.Count; i++)
            {
                var row = i / _columns;
                var col = i % _columns;
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
}