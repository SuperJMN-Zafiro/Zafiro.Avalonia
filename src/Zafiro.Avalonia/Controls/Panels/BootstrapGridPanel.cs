using System.ComponentModel;

namespace Zafiro.Avalonia.Controls.Panels;

public class BootstrapGridPanel : Panel
{
    static BootstrapGridPanel()
    {
        AffectsArrange<BootstrapGridPanel>(
            MaxColumnsProperty,
            GutterProperty,
            FluidContainerProperty);

        AffectsMeasure<BootstrapGridPanel>(
            SmallBreakpointProperty,
            MediumBreakpointProperty,
            LargeBreakpointProperty,
            ExtraLargeBreakpointProperty,
            XXLBreakpointProperty,
            MaxColumnsProperty,
            GutterProperty,
            FluidContainerProperty);

        // Attached properties affect parent layout
        ColProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColSmProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColMdProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColLgProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColXlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        ColXxlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetSmProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetMdProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetLgProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OffsetXlProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        OrderProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
        RowBreakProperty.Changed.AddClassHandler<Control>((c, e) => InvalidateParentLayout(c));
    }

    private static void InvalidateParentLayout(Control control)
    {
        if (control?.Parent is BootstrapGridPanel panel)
        {
            panel.InvalidateMeasure();
            panel.InvalidateArrange();
        }
    }

    private Breakpoint GetCurrentBreakpoint(double width)
    {
        if (width >= XXLBreakpoint) return Breakpoint.Xxl;
        if (width >= ExtraLargeBreakpoint) return Breakpoint.Xl;
        if (width >= LargeBreakpoint) return Breakpoint.Lg;
        if (width >= MediumBreakpoint) return Breakpoint.Md;
        if (width >= SmallBreakpoint) return Breakpoint.Sm;
        return Breakpoint.Xs;
    }

    private int GetColumnSpan(Control child, Breakpoint breakpoint)
    {
        int span = 0;

        // Start with base col value
        span = GetCol(child);

        // Override with breakpoint-specific values if set (0 means not set)
        switch (breakpoint)
        {
            case Breakpoint.Xxl:
                if (GetColXxl(child) > 0) span = GetColXxl(child);
                else goto case Breakpoint.Xl;
                break;
            case Breakpoint.Xl:
                if (GetColXl(child) > 0) span = GetColXl(child);
                else goto case Breakpoint.Lg;
                break;
            case Breakpoint.Lg:
                if (GetColLg(child) > 0) span = GetColLg(child);
                else goto case Breakpoint.Md;
                break;
            case Breakpoint.Md:
                if (GetColMd(child) > 0) span = GetColMd(child);
                else goto case Breakpoint.Sm;
                break;
            case Breakpoint.Sm:
                if (GetColSm(child) > 0) span = GetColSm(child);
                break;
        }

        return Math.Min(Math.Max(1, span), MaxColumns);
    }

    private int GetOffset(Control child, Breakpoint breakpoint)
    {
        int offset = GetOffset(child);

        switch (breakpoint)
        {
            case Breakpoint.Xl:
            case Breakpoint.Xxl:
                if (GetOffsetXl(child) > 0) offset = GetOffsetXl(child);
                else goto case Breakpoint.Lg;
                break;
            case Breakpoint.Lg:
                if (GetOffsetLg(child) > 0) offset = GetOffsetLg(child);
                else goto case Breakpoint.Md;
                break;
            case Breakpoint.Md:
                if (GetOffsetMd(child) > 0) offset = GetOffsetMd(child);
                else goto case Breakpoint.Sm;
                break;
            case Breakpoint.Sm:
                if (GetOffsetSm(child) > 0) offset = GetOffsetSm(child);
                break;
        }

        return Math.Min(Math.Max(0, offset), MaxColumns - 1);
    }

    private double GetContainerWidth(double availableWidth)
    {
        if (FluidContainer)
            return availableWidth;

        // Fixed max widths per breakpoint (Bootstrap defaults)
        var breakpoint = GetCurrentBreakpoint(availableWidth);
        return breakpoint switch
        {
            Breakpoint.Sm => Math.Min(540, availableWidth),
            Breakpoint.Md => Math.Min(720, availableWidth),
            Breakpoint.Lg => Math.Min(960, availableWidth),
            Breakpoint.Xl => Math.Min(1140, availableWidth),
            Breakpoint.Xxl => Math.Min(1320, availableWidth),
            _ => availableWidth
        };
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Children.Count == 0)
            return new Size();

        var containerWidth = GetContainerWidth(availableSize.Width);
        var breakpoint = GetCurrentBreakpoint(availableSize.Width);
        var columnWidth = (containerWidth - Gutter * (MaxColumns - 1)) / MaxColumns;
        var halfGutter = Gutter / 2;

        // Sort children by order
        var orderedChildren = Children
            .OrderBy(c => GetOrder(c))
            .ToList();

        var rows = new List<List<Control>>();
        var currentRow = new List<Control>();
        var currentColumn = 0;

        foreach (var child in orderedChildren)
        {
            if (GetRowBreak(child) && currentRow.Count > 0)
            {
                rows.Add(currentRow);
                currentRow = new List<Control>();
                currentColumn = 0;
            }

            var span = GetColumnSpan(child, breakpoint);
            var offset = GetOffset(child, breakpoint);

            currentColumn += offset;

            if (currentColumn + span > MaxColumns && currentRow.Count > 0)
            {
                rows.Add(currentRow);
                currentRow = new List<Control>();
                currentColumn = offset;
            }

            currentRow.Add(child);
            currentColumn += span;

            // Measure child
            var childWidth = columnWidth * span + Gutter * (span - 1);
            child.Measure(new Size(childWidth, availableSize.Height));
        }

        if (currentRow.Count > 0)
            rows.Add(currentRow);

        // Calculate total height
        var totalHeight = rows.Sum(row => row.Max(c => c.DesiredSize.Height)) +
                          Math.Max(0, (rows.Count - 1) * Gutter);

        return new Size(containerWidth, Math.Min(totalHeight, availableSize.Height));
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (Children.Count == 0)
            return finalSize;

        var containerWidth = GetContainerWidth(finalSize.Width);
        var breakpoint = GetCurrentBreakpoint(finalSize.Width);
        var columnWidth = (containerWidth - Gutter * (MaxColumns - 1)) / MaxColumns;
        var halfGutter = Gutter / 2;

        // Center container if not fluid
        var containerOffset = FluidContainer ? 0 : (finalSize.Width - containerWidth) / 2;

        var orderedChildren = Children
            .OrderBy(c => GetOrder(c))
            .ToList();

        var currentY = 0.0;
        var currentRow = new List<Control>();
        var currentColumn = 0;
        var rowHeight = 0.0;

        void ArrangeRow()
        {
            if (currentRow.Count == 0) return;

            var col = 0;
            foreach (var child in currentRow)
            {
                var span = GetColumnSpan(child, breakpoint);
                var offset = GetOffset(child, breakpoint);

                col += offset;

                var x = containerOffset + col * (columnWidth + Gutter);
                var width = columnWidth * span + Gutter * (span - 1);

                child.Arrange(new Rect(x, currentY, width, rowHeight));

                col += span;
            }

            currentY += rowHeight + Gutter;
            currentRow.Clear();
            currentColumn = 0;
            rowHeight = 0;
        }

        foreach (var child in orderedChildren)
        {
            if (GetRowBreak(child) && currentRow.Count > 0)
            {
                ArrangeRow();
            }

            var span = GetColumnSpan(child, breakpoint);
            var offset = GetOffset(child, breakpoint);

            currentColumn += offset;

            if (currentColumn + span > MaxColumns && currentRow.Count > 0)
            {
                ArrangeRow();
                currentColumn = offset;
            }

            currentRow.Add(child);
            currentColumn += span;
            rowHeight = Math.Max(rowHeight, child.DesiredSize.Height);
        }

        // Arrange last row
        if (currentRow.Count > 0)
        {
            ArrangeRow();
        }

        return finalSize;
    }

    private enum Breakpoint
    {
        Xs,
        Sm,
        Md,
        Lg,
        Xl,
        Xxl
    }

    #region Styled Properties

    // Breakpoints
    public static readonly StyledProperty<double> SmallBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(SmallBreakpoint), 576);

    public static readonly StyledProperty<double> MediumBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(MediumBreakpoint), 768);

    public static readonly StyledProperty<double> LargeBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(LargeBreakpoint), 992);

    public static readonly StyledProperty<double> ExtraLargeBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(ExtraLargeBreakpoint), 1200);

    public static readonly StyledProperty<double> XXLBreakpointProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(XXLBreakpoint), 1400);

    // Grid configuration
    public static readonly StyledProperty<int> MaxColumnsProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, int>(nameof(MaxColumns), 12);

    public static readonly StyledProperty<double> GutterProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, double>(nameof(Gutter), 24);

    public static readonly StyledProperty<bool> FluidContainerProperty =
        AvaloniaProperty.Register<BootstrapGridPanel, bool>(nameof(FluidContainer), false);

    #endregion

    #region Attached Properties for Children

    // Column spans for different breakpoints
    public static readonly AttachedProperty<int> ColProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("Col", 12);

    public static readonly AttachedProperty<int> ColSmProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColSm", 0);

    public static readonly AttachedProperty<int> ColMdProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColMd", 0);

    public static readonly AttachedProperty<int> ColLgProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColLg", 0);

    public static readonly AttachedProperty<int> ColXlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColXl", 0);

    public static readonly AttachedProperty<int> ColXxlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("ColXxl", 0);

    // Offset properties
    public static readonly AttachedProperty<int> OffsetProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("Offset", 0);

    public static readonly AttachedProperty<int> OffsetSmProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetSm", 0);

    public static readonly AttachedProperty<int> OffsetMdProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetMd", 0);

    public static readonly AttachedProperty<int> OffsetLgProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetLg", 0);

    public static readonly AttachedProperty<int> OffsetXlProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("OffsetXl", 0);

    // Order property
    public static readonly AttachedProperty<int> OrderProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, int>("Order", 0);

    // Row break property
    public static readonly AttachedProperty<bool> RowBreakProperty =
        AvaloniaProperty.RegisterAttached<BootstrapGridPanel, Control, bool>("RowBreak", false);

    #endregion

    #region Public Properties

    [Category("Layout")]
    public double SmallBreakpoint
    {
        get => GetValue(SmallBreakpointProperty);
        set => SetValue(SmallBreakpointProperty, value);
    }

    [Category("Layout")]
    public double MediumBreakpoint
    {
        get => GetValue(MediumBreakpointProperty);
        set => SetValue(MediumBreakpointProperty, value);
    }

    [Category("Layout")]
    public double LargeBreakpoint
    {
        get => GetValue(LargeBreakpointProperty);
        set => SetValue(LargeBreakpointProperty, value);
    }

    [Category("Layout")]
    public double ExtraLargeBreakpoint
    {
        get => GetValue(ExtraLargeBreakpointProperty);
        set => SetValue(ExtraLargeBreakpointProperty, value);
    }

    [Category("Layout")]
    public double XXLBreakpoint
    {
        get => GetValue(XXLBreakpointProperty);
        set => SetValue(XXLBreakpointProperty, value);
    }

    [Category("Layout")]
    public int MaxColumns
    {
        get => GetValue(MaxColumnsProperty);
        set => SetValue(MaxColumnsProperty, value);
    }

    [Category("Layout")]
    public double Gutter
    {
        get => GetValue(GutterProperty);
        set => SetValue(GutterProperty, value);
    }

    [Category("Layout")]
    public bool FluidContainer
    {
        get => GetValue(FluidContainerProperty);
        set => SetValue(FluidContainerProperty, value);
    }

    #endregion

    #region Static Methods for Attached Properties

    public static void SetCol(Control element, int value) => element.SetValue(ColProperty, value);
    public static int GetCol(Control element) => element.GetValue(ColProperty);

    public static void SetColSm(Control element, int value) => element.SetValue(ColSmProperty, value);
    public static int GetColSm(Control element) => element.GetValue(ColSmProperty);

    public static void SetColMd(Control element, int value) => element.SetValue(ColMdProperty, value);
    public static int GetColMd(Control element) => element.GetValue(ColMdProperty);

    public static void SetColLg(Control element, int value) => element.SetValue(ColLgProperty, value);
    public static int GetColLg(Control element) => element.GetValue(ColLgProperty);

    public static void SetColXl(Control element, int value) => element.SetValue(ColXlProperty, value);
    public static int GetColXl(Control element) => element.GetValue(ColXlProperty);

    public static void SetColXxl(Control element, int value) => element.SetValue(ColXxlProperty, value);
    public static int GetColXxl(Control element) => element.GetValue(ColXxlProperty);

    public static void SetOffset(Control element, int value) => element.SetValue(OffsetProperty, value);
    public static int GetOffset(Control element) => element.GetValue(OffsetProperty);

    public static void SetOffsetSm(Control element, int value) => element.SetValue(OffsetSmProperty, value);
    public static int GetOffsetSm(Control element) => element.GetValue(OffsetSmProperty);

    public static void SetOffsetMd(Control element, int value) => element.SetValue(OffsetMdProperty, value);
    public static int GetOffsetMd(Control element) => element.GetValue(OffsetMdProperty);

    public static void SetOffsetLg(Control element, int value) => element.SetValue(OffsetLgProperty, value);
    public static int GetOffsetLg(Control element) => element.GetValue(OffsetLgProperty);

    public static void SetOffsetXl(Control element, int value) => element.SetValue(OffsetXlProperty, value);
    public static int GetOffsetXl(Control element) => element.GetValue(OffsetXlProperty);

    public static void SetOrder(Control element, int value) => element.SetValue(OrderProperty, value);
    public static int GetOrder(Control element) => element.GetValue(OrderProperty);

    public static void SetRowBreak(Control element, bool value) => element.SetValue(RowBreakProperty, value);
    public static bool GetRowBreak(Control element) => element.GetValue(RowBreakProperty);

    #endregion
}

// Helper class for easier configuration
public static class ResponsiveGrid
{
    public static T Col<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetCol(control, columns);
        return control;
    }

    public static T ColSm<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColSm(control, columns);
        return control;
    }

    public static T ColMd<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColMd(control, columns);
        return control;
    }

    public static T ColLg<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColLg(control, columns);
        return control;
    }

    public static T ColXl<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColXl(control, columns);
        return control;
    }

    public static T ColXxl<T>(this T control, int columns) where T : Control
    {
        BootstrapGridPanel.SetColXxl(control, columns);
        return control;
    }

    public static T Offset<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffset(control, offset);
        return control;
    }

    public static T OffsetSm<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetSm(control, offset);
        return control;
    }

    public static T OffsetMd<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetMd(control, offset);
        return control;
    }

    public static T OffsetLg<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetLg(control, offset);
        return control;
    }

    public static T OffsetXl<T>(this T control, int offset) where T : Control
    {
        BootstrapGridPanel.SetOffsetXl(control, offset);
        return control;
    }

    public static T Order<T>(this T control, int order) where T : Control
    {
        BootstrapGridPanel.SetOrder(control, order);
        return control;
    }

    public static T RowBreak<T>(this T control, bool breakRow = true) where T : Control
    {
        BootstrapGridPanel.SetRowBreak(control, breakRow);
        return control;
    }
}