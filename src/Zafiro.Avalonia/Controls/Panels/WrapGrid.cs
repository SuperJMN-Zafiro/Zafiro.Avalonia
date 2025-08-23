using System.Buffers;
using System.Collections.Specialized;

namespace Zafiro.Avalonia.Controls.Panels;

/// <summary>
/// Optimized WrapGrid: lays out children in rows with Grid-like width semantics (Pixel, Auto, Star).
/// </summary>
public class WrapGrid : Panel
{
    private const double Tolerance = 0.05; // layout reuse tolerance y umbral de slack
    private const int MaxRowPoolSize = 256;

    private const int MaxItemPoolSize = 4096;

    // Attached properties
    public static readonly AttachedProperty<GridLength> PreferredWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>("PreferredWidth", new GridLength(1, GridUnitType.Auto));

    public static readonly AttachedProperty<GridLength> MinPreferredWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>("MinPreferredWidth", new GridLength(0, GridUnitType.Pixel));

    public static readonly AttachedProperty<GridLength> FillWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, GridLength>("FillWidth", new GridLength(0, GridUnitType.Pixel));

    public static readonly AttachedProperty<double> WrapMinWidthProperty =
        AvaloniaProperty.RegisterAttached<WrapGrid, Control, double>("WrapMinWidth", 0d);

    public static readonly StyledProperty<double> ColumnSpacingProperty =
        AvaloniaProperty.Register<WrapGrid, double>(nameof(ColumnSpacing), 0d);

    public static readonly StyledProperty<double> RowSpacingProperty =
        AvaloniaProperty.Register<WrapGrid, double>(nameof(RowSpacing), 0d);

    // Cache Auto
    private readonly Dictionary<Control, double> autoWidthCache = new();
    private readonly Stack<Item> itemPool = new();

    // Pools (UI thread)
    private readonly Stack<Row> rowPool = new();
    private readonly List<Row> rowsBuffer = new(16);
    private readonly Dictionary<Control, IDisposable> visibilitySubscriptions = new();
    private double cachedHeightConstraint = double.NaN;

    // Layout cache
    private List<Row>? cachedRows;
    private double cachedWidthConstraint = double.NaN;
    private bool cacheValid;

    static WrapGrid()
    {
        ColumnSpacingProperty.Changed.AddClassHandler<WrapGrid>((g, _) => g.InvalidateLayoutCache());
        RowSpacingProperty.Changed.AddClassHandler<WrapGrid>((g, _) => g.InvalidateLayoutCache());
    }

    public WrapGrid()
    {
        Children.CollectionChanged += OnChildrenChanged;
    }

    public double ColumnSpacing
    {
        get => GetValue(ColumnSpacingProperty);
        set => SetValue(ColumnSpacingProperty, value);
    }

    public double RowSpacing
    {
        get => GetValue(RowSpacingProperty);
        set => SetValue(RowSpacingProperty, value);
    }

    private void InvalidateLayoutCache()
    {
        cacheValid = false;
    }

    private void OnChildrenChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        cacheValid = false;
        if (e.OldItems != null)
        {
            foreach (var item in e.OldItems)
            {
                if (item is Control c)
                {
                    autoWidthCache.Remove(c);
                    if (visibilitySubscriptions.TryGetValue(c, out var disp))
                    {
                        disp.Dispose();
                        visibilitySubscriptions.Remove(c);
                    }
                }
            }
        }

        if (e.NewItems != null)
        {
            foreach (var item in e.NewItems)
            {
                if (item is Control c)
                {
                    var sub = c.GetObservable(IsVisibleProperty).Subscribe(_ =>
                    {
                        autoWidthCache.Remove(c);
                        InvalidateLayoutCache();
                        InvalidateMeasure();
                    });
                    visibilitySubscriptions[c] = sub;
                }
            }
        }
    }

    public static void SetPreferredWidth(AvaloniaObject target, GridLength value) => target.SetValue(PreferredWidthProperty, value);
    public static GridLength GetPreferredWidth(AvaloniaObject target) => target.GetValue(PreferredWidthProperty);
    public static void SetMinPreferredWidth(AvaloniaObject target, GridLength value) => target.SetValue(MinPreferredWidthProperty, value);
    public static GridLength GetMinPreferredWidth(AvaloniaObject target) => target.GetValue(MinPreferredWidthProperty);
    public static void SetFillWidth(AvaloniaObject target, GridLength value) => target.SetValue(FillWidthProperty, value);
    public static GridLength GetFillWidth(AvaloniaObject target) => target.GetValue(FillWidthProperty);
    public static void SetWrapMinWidth(AvaloniaObject target, double value) => target.SetValue(WrapMinWidthProperty, value);
    public static double GetWrapMinWidth(AvaloniaObject target) => target.GetValue(WrapMinWidthProperty);

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        foreach (var disp in visibilitySubscriptions.Values) disp.Dispose();
        visibilitySubscriptions.Clear();
        autoWidthCache.Clear();
        ReleaseRows(rowsBuffer);
        rowsBuffer.Clear();
        cachedRows = null;
        cacheValid = false;
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0) return new Size();

        double availableRowWidth = double.IsInfinity(availableSize.Width) ? double.PositiveInfinity : availableSize.Width;
        double colSpacing = ColumnSpacing;
        double rowSpacing = RowSpacing;

        if (!cacheValid || !NearlyEqual(availableRowWidth, cachedWidthConstraint) || !NearlyEqual(availableSize.Height, cachedHeightConstraint))
        {
            var rows = BuildRows(availableRowWidth, availableSize.Height);
            foreach (var row in rows)
                AllocateRowWidths(row, availableRowWidth, availableSize.Height, colSpacing, true);
            cachedRows = rows;
            cachedWidthConstraint = availableRowWidth;
            cachedHeightConstraint = availableSize.Height;
            cacheValid = true;
        }

        var usedRows = cachedRows!;
        double totalHeight = 0;
        double maxWidth = 0;
        bool first = true;
        foreach (var r in usedRows)
        {
            if (!first) totalHeight += rowSpacing;
            else first = false;
            totalHeight += r.Height;
            if (r.Width > maxWidth) maxWidth = r.Width;
        }

        double finalWidth = double.IsInfinity(availableSize.Width) ? maxWidth : Math.Min(maxWidth, availableSize.Width);
        double finalHeight = double.IsInfinity(availableSize.Height) ? totalHeight : Math.Min(totalHeight, availableSize.Height);
        return new Size(finalWidth, finalHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0) return finalSize;

        double availableRowWidth = finalSize.Width;
        double colSpacing = ColumnSpacing;
        double rowSpacing = RowSpacing;

        bool canReuse = cacheValid && cachedRows != null && NearlyEqual(availableRowWidth, cachedWidthConstraint) && NearlyEqual(finalSize.Height, cachedHeightConstraint);
        List<Row> rowsToUse;
        if (canReuse)
        {
            rowsToUse = cachedRows!;
        }
        else
        {
            rowsToUse = BuildRows(availableRowWidth, finalSize.Height);
            foreach (var row in rowsToUse)
                AllocateRowWidths(row, availableRowWidth, finalSize.Height, colSpacing, true);
        }

        double y = 0;
        bool first = true;
        foreach (var row in rowsToUse)
        {
            if (!first) y += rowSpacing;
            else first = false;
            double x = 0;
            int i = 0;
            foreach (var item in row.Items)
            {
                if (i > 0) x += colSpacing;
                item.Child.Arrange(new Rect(x, y, item.AllocatedWidth, row.Height));
                x += item.AllocatedWidth;
                i++;
            }

            y += row.Height;
        }

        return finalSize;
    }

    private static double GetItemMinContribution(Item item) => Math.Max(GetBasePreferredMin(item), ComputeMinPreferred(item));

    private static double GetBasePreferredMin(Item item)
    {
        if (item.Preferred.IsAbsolute) return item.Preferred.Value;
        if (item.WrapMinWidth > 0) return item.WrapMinWidth;
        if (item.Preferred.IsAuto) return item.AutoDesiredWidth;
        return 0;
    }

    private static double ComputeMinPreferred(Item item)
    {
        var mp = item.MinPreferred;
        if (mp.IsAbsolute) return mp.Value;
        if (mp.IsAuto) return item.AutoDesiredWidth;
        return 0;
    }

    private bool NearlyEqual(double a, double b)
    {
        if (double.IsNaN(a) || double.IsNaN(b)) return false;
        if (double.IsInfinity(a) && double.IsInfinity(b)) return true;
        return Math.Abs(a - b) <= Tolerance;
    }

    private List<Row> BuildRows(double availableRowWidth, double availableHeight)
    {
        if (!ReferenceEquals(cachedRows, rowsBuffer)) ReleaseRows(rowsBuffer);
        rowsBuffer.Clear();
        var current = AcquireRow();
        rowsBuffer.Add(current);
        double currentMin = 0;
        double colSpacing = ColumnSpacing;
        foreach (var child in Children)
        {
            if (child is not Control c || !c.IsVisible) continue;
            var item = CreateItem(c, availableHeight);
            double itemMin = GetItemMinContribution(item);
            double required = currentMin + (current.Items.Count > 0 ? colSpacing : 0) + itemMin;
            if (!double.IsInfinity(availableRowWidth) && current.Items.Count > 0 && required > availableRowWidth)
            {
                current = AcquireRow();
                rowsBuffer.Add(current);
                currentMin = 0;
                required = itemMin;
            }

            if (current.Items.Count > 0) currentMin += colSpacing;
            currentMin += itemMin;
            current.Items.Add(item);
        }

        return rowsBuffer;
    }

    private Item CreateItem(Control c, double availableHeight)
    {
        var preferred = GetPreferredWidth(c);
        var minPreferred = GetMinPreferredWidth(c);
        var fill = GetFillWidth(c);
        double autoDesired = 0;
        if (preferred.IsAuto || fill.IsAuto)
        {
            if (!autoWidthCache.TryGetValue(c, out autoDesired))
            {
                c.Measure(new Size(double.PositiveInfinity, availableHeight));
                autoDesired = c.DesiredSize.Width;
                autoWidthCache[c] = autoDesired;
            }
        }

        var item = AcquireItem();
        item.Child = c;
        item.Preferred = preferred;
        item.MinPreferred = minPreferred;
        item.Fill = fill;
        item.AutoDesiredWidth = autoDesired;
        item.WrapMinWidth = GetWrapMinWidth(c);
        item.AllocatedWidth = 0;
        return item;
    }

    private void AllocateRowWidths(Row row, double availableRowWidth, double availableHeight, double spacing, bool measureChildren)
    {
        if (row.Items.Count == 0)
        {
            row.Height = 0;
            row.Width = 0;
            return;
        }

        double fixedSum = 0;
        double totalStar = 0;
        foreach (var it in row.Items)
        {
            var pref = it.Preferred;
            if (pref.IsAbsolute) fixedSum += pref.Value;
            else if (pref.IsAuto) fixedSum += it.AutoDesiredWidth;
            else if (pref.IsStar) totalStar += Math.Max(pref.Value, 0.0001);
        }

        int gaps = row.Items.Count - 1;
        double spacingTotal = gaps > 0 ? gaps * spacing : 0;
        double leftover = double.IsInfinity(availableRowWidth) ? fixedSum : availableRowWidth - fixedSum - spacingTotal;
        if (leftover < 0) leftover = 0;
        foreach (var it in row.Items)
        {
            double w;
            if (it.Preferred.IsAbsolute) w = it.Preferred.Value;
            else if (it.Preferred.IsAuto) w = it.AutoDesiredWidth;
            else w = totalStar > 0 ? leftover * (it.Preferred.Value / totalStar) : 0;
            double enforcedMin = Math.Max(ComputeMinPreferred(it), it.WrapMinWidth > 0 ? it.WrapMinWidth : 0);
            if (w < enforcedMin) w = enforcedMin;
            it.AllocatedWidth = w;
        }

        double baseRowWidth = spacingTotal;
        foreach (var it in row.Items) baseRowWidth += it.AllocatedWidth;
        if (!double.IsInfinity(availableRowWidth) && baseRowWidth > availableRowWidth + Tolerance)
        {
            double over = baseRowWidth - availableRowWidth;
            var buffer = ArrayPool<Item>.Shared.Rent(row.Items.Count);
            int count = 0;
            double totalSlack = 0;
            foreach (var it in row.Items)
            {
                if (it.Preferred.IsAbsolute) continue;
                double min = Math.Max(ComputeMinPreferred(it), it.WrapMinWidth > 0 ? it.WrapMinWidth : 0);
                double slack = it.AllocatedWidth - min;
                if (slack > Tolerance)
                {
                    buffer[count++] = it;
                    totalSlack += slack;
                }
            }

            if (totalSlack > 0)
            {
                for (int i = 0; i < count && over > Tolerance; i++)
                {
                    var it = buffer[i];
                    double min = Math.Max(ComputeMinPreferred(it), it.WrapMinWidth > 0 ? it.WrapMinWidth : 0);
                    double slack = it.AllocatedWidth - min;
                    if (slack <= 0) continue;
                    double reduce = Math.Min(slack, over * (slack / totalSlack));
                    it.AllocatedWidth -= reduce;
                    over -= reduce;
                }
            }

            ArrayPool<Item>.Shared.Return(buffer);
            baseRowWidth = spacingTotal;
            foreach (var it in row.Items) baseRowWidth += it.AllocatedWidth;
        }

        double spare = double.IsInfinity(availableRowWidth) ? 0 : Math.Max(0, availableRowWidth - baseRowWidth);
        if (spare > Tolerance)
        {
            var fillCandidates = ArrayPool<(Item item, double need)>.Shared.Rent(row.Items.Count);
            int needed = 0;
            double totalNeed = 0;
            foreach (var it in row.Items)
            {
                var fill = it.Fill;
                if (fill.GridUnitType == GridUnitType.Pixel && fill.Value > it.AllocatedWidth)
                {
                    double need = fill.Value - it.AllocatedWidth;
                    if (need > Tolerance)
                    {
                        fillCandidates[needed++] = (it, need);
                        totalNeed += need;
                    }
                }
                else if (fill.GridUnitType == GridUnitType.Auto)
                {
                    double target = Math.Max(it.AutoDesiredWidth, it.AllocatedWidth);
                    double need = target - it.AllocatedWidth;
                    if (need > Tolerance)
                    {
                        fillCandidates[needed++] = (it, need);
                        totalNeed += need;
                    }
                }
            }

            if (totalNeed > 0)
            {
                for (int i = 0; i < needed && spare > Tolerance; i++)
                {
                    var pair = fillCandidates[i];
                    double grant = Math.Min(pair.need, spare * (pair.need / totalNeed));
                    pair.item.AllocatedWidth += grant;
                    spare -= grant;
                }
            }

            ArrayPool<(Item, double)>.Shared.Return(fillCandidates);
            if (spare > Tolerance)
            {
                double starFillSum = 0;
                foreach (var it in row.Items)
                    if (it.Fill.IsStar)
                        starFillSum += Math.Max(it.Fill.Value, 0.0001);
                if (starFillSum > 0)
                {
                    foreach (var it in row.Items)
                        if (it.Fill.IsStar)
                            it.AllocatedWidth += spare * (it.Fill.Value / starFillSum);
                }
            }
        }

        double rowHeight = 0;
        foreach (var it in row.Items)
        {
            it.Child.Measure(new Size(it.AllocatedWidth, availableHeight));
            double h = it.Child.DesiredSize.Height;
            if (h > rowHeight) rowHeight = h;
        }

        row.Height = rowHeight;
        row.Width = spacingTotal;
        foreach (var it in row.Items) row.Width += it.AllocatedWidth;
    }

    public void InvalidateAutoWidthCache()
    {
        if (autoWidthCache.Count == 0) return;
        autoWidthCache.Clear();
        cacheValid = false;
        InvalidateMeasure();
    }

    private void ReleaseRows(List<Row> rows)
    {
        foreach (var r in rows)
        {
            foreach (var it in r.Items)
            {
                if (itemPool.Count < MaxItemPoolSize)
                {
                    it.Child = null!;
                    itemPool.Push(it);
                }
            }

            r.Items.Clear();
            r.Height = 0;
            r.Width = 0;
            if (rowPool.Count < MaxRowPoolSize) rowPool.Push(r);
        }
    }

    private Row AcquireRow()
    {
        if (rowPool.Count > 0)
        {
            var r = rowPool.Pop();
            r.Items.Clear();
            r.Height = 0;
            r.Width = 0;
            return r;
        }

        return new Row();
    }

    private Item AcquireItem()
    {
        if (itemPool.Count > 0) return itemPool.Pop();
        return new Item();
    }

    private class Row
    {
        public double Height;
        public double Width;
        public List<Item> Items { get; } = new(8);
    }

    private class Item
    {
        public double AllocatedWidth;
        public double AutoDesiredWidth;
        public Control Child = null!;
        public GridLength Fill;
        public GridLength MinPreferred;
        public GridLength Preferred;
        public double WrapMinWidth;
    }
}