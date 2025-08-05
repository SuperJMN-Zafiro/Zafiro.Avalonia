using CSharpFunctionalExtensions;
using Avalonia.Layout;

namespace Zafiro.Avalonia.Controls;

public enum FlexWrap
{
    NoWrap,
    Wrap
}

public enum FlexJustify
{
    Start,
    Center,
    End,
    SpaceBetween,
    SpaceAround,
    SpaceEvenly
}

public enum FlexAlign
{
    Start,
    Center,
    End,
    Stretch
}

public class FlexPanel : Panel
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<FlexPanel, Orientation>(nameof(Orientation), Orientation.Horizontal);

    public static readonly StyledProperty<FlexWrap> WrapProperty =
        AvaloniaProperty.Register<FlexPanel, FlexWrap>(nameof(Wrap));

    public static readonly StyledProperty<FlexJustify> JustifyContentProperty =
        AvaloniaProperty.Register<FlexPanel, FlexJustify>(nameof(JustifyContent));

    public static readonly StyledProperty<FlexAlign> AlignItemsProperty =
        AvaloniaProperty.Register<FlexPanel, FlexAlign>(nameof(AlignItems), FlexAlign.Stretch);

    public static readonly AttachedProperty<double> GrowProperty =
        AvaloniaProperty.RegisterAttached<FlexPanel, Control, double>("Grow", 0d);

    public static readonly AttachedProperty<FlexAlign?> AlignSelfProperty =
        AvaloniaProperty.RegisterAttached<FlexPanel, Control, FlexAlign?>("AlignSelf");

    static FlexPanel()
    {
        AffectsMeasure<FlexPanel>(OrientationProperty, WrapProperty, JustifyContentProperty, AlignItemsProperty, GrowProperty, AlignSelfProperty);
    }

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public FlexWrap Wrap
    {
        get => GetValue(WrapProperty);
        set => SetValue(WrapProperty, value);
    }

    public FlexJustify JustifyContent
    {
        get => GetValue(JustifyContentProperty);
        set => SetValue(JustifyContentProperty, value);
    }

    public FlexAlign AlignItems
    {
        get => GetValue(AlignItemsProperty);
        set => SetValue(AlignItemsProperty, value);
    }

    public static void SetGrow(AvaloniaObject target, double value) => target.SetValue(GrowProperty, value);

    public static double GetGrow(AvaloniaObject target) => target.GetValue(GrowProperty);

    public static void SetAlignSelf(AvaloniaObject target, FlexAlign? value) => target.SetValue(AlignSelfProperty, value);

    public static FlexAlign? GetAlignSelf(AvaloniaObject target) => target.GetValue(AlignSelfProperty);

    protected override Size MeasureOverride(Size availableSize)
    {
        foreach (var child in Children)
        {
            child.Measure(Size.Infinity);
        }

        var lines = CreateLines(Orientation == Orientation.Horizontal ? availableSize.Width : availableSize.Height);

        var panelMain = lines.Max(l => l.Main);
        var panelCross = lines.Sum(l => l.Cross);

        return Orientation == Orientation.Horizontal
            ? new Size(panelMain, panelCross)
            : new Size(panelCross, panelMain);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        var containerMain = Orientation == Orientation.Horizontal ? finalSize.Width : finalSize.Height;
        var lines = CreateLines(containerMain);

        double crossPos = 0;
        foreach (var line in lines)
        {
            DistributeGrowth(line, containerMain);

            var remaining = containerMain - line.Main;
            var (offset, spacing) = GetMainOffsets(remaining, line.Children.Count);

            double mainPos = offset;
            foreach (var item in line.Children)
            {
                var align = Maybe.From(GetAlignSelf(item.Control))
                    .Match(a => a ?? AlignItems, () => AlignItems);
                var (crossOffset, crossSize) = align switch
                {
                    FlexAlign.Center => ((line.Cross - item.Cross) / 2, item.Cross),
                    FlexAlign.End => (line.Cross - item.Cross, item.Cross),
                    FlexAlign.Stretch => (0, line.Cross),
                    _ => (0, item.Cross)
                };

                if (Orientation == Orientation.Horizontal)
                {
                    item.Control.Arrange(new Rect(mainPos, crossPos + crossOffset, item.Main, crossSize));
                }
                else
                {
                    item.Control.Arrange(new Rect(crossPos + crossOffset, mainPos, crossSize, item.Main));
                }

                mainPos += item.Main + spacing;
            }

            crossPos += line.Cross;
        }

        return finalSize;
    }

    private (double offset, double spacing) GetMainOffsets(double remaining, int count)
    {
        return JustifyContent switch
        {
            FlexJustify.Center => (remaining / 2, 0),
            FlexJustify.End => (remaining, 0),
            FlexJustify.SpaceBetween => (0, count > 1 ? remaining / (count - 1) : 0),
            FlexJustify.SpaceAround => (remaining / count / 2, remaining / count),
            FlexJustify.SpaceEvenly => (remaining / (count + 1), remaining / (count + 1)),
            _ => (0, 0)
        };
    }

    private void DistributeGrowth(Line line, double containerMain)
    {
        var remaining = containerMain - line.Main;
        if (remaining <= 0)
        {
            return;
        }

        var totalGrow = line.Children.Sum(c => c.Grow);
        if (totalGrow <= 0)
        {
            return;
        }

        foreach (var child in line.Children)
        {
            var extra = remaining * child.Grow / totalGrow;
            child.Main += extra;
        }

        line.Main = containerMain;
    }

    private List<Line> CreateLines(double containerMain)
    {
        var result = new List<Line>();
        var line = new Line();

        foreach (var child in Children)
        {
            var size = child.DesiredSize;
            var main = Orientation == Orientation.Horizontal ? size.Width : size.Height;
            var cross = Orientation == Orientation.Horizontal ? size.Height : size.Width;
            var grow = GetGrow(child);

            if (Wrap == FlexWrap.Wrap && line.Main + main > containerMain && line.Children.Any())
            {
                result.Add(line);
                line = new Line();
            }

            line.Add(new ChildInfo(child, main, cross, grow));
        }

        result.Add(line);
        return result;
    }

    private class Line
    {
        public List<ChildInfo> Children { get; } = new();
        public double Main;
        public double Cross;

        public void Add(ChildInfo child)
        {
            Children.Add(child);
            Main += child.Main;
            Cross = Math.Max(Cross, child.Cross);
        }
    }

    private class ChildInfo
    {
        public ChildInfo(Control control, double main, double cross, double grow)
        {
            Control = control;
            Main = main;
            Cross = cross;
            Grow = grow;
        }

        public Control Control { get; }
        public double Main { get; set; }
        public double Cross { get; }
        public double Grow { get; }
    }
}

