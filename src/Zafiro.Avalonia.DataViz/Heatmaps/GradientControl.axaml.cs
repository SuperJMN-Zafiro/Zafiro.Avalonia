using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using ReactiveUI;

namespace Zafiro.Avalonia.DataViz.Heatmaps;

public class GradientControl : RangeBase
{
    public GradientControl()
    {
        this.WhenAnyValue(x => x.Minimum, x => x.Maximum, x => x.NumberOfDivisions, (min, max, tn) =>
        {
            return Enumerable.Range(0, tn + 1).Select(i => (max - min) * i / tn);
        }).BindTo(this, x => x.Ticks);
    }
    
    public static readonly StyledProperty<IEnumerable<Color>> ColorListProperty = AvaloniaProperty.Register<GradientControl, IEnumerable<Color>>(
        nameof(ColorList));
    
    private IEnumerable<double> ticks;

    public static readonly DirectProperty<GradientControl, IEnumerable<double>> TicksProperty = AvaloniaProperty.RegisterDirect<GradientControl, IEnumerable<double>>(
        nameof(Ticks), o => o.Ticks, (o, v) => o.Ticks = v);

    public static readonly StyledProperty<int> NumberOfDivisionsProperty = AvaloniaProperty.Register<GradientControl, int>(
        nameof(NumberOfDivisions), 3);

    public int NumberOfDivisions
    {
        get => GetValue(NumberOfDivisionsProperty);
        set => SetValue(NumberOfDivisionsProperty, value);
    }

    public IEnumerable<double> Ticks
    {
        get => ticks;
        private set => SetAndRaise(TicksProperty, ref ticks, value);
    }

    public IEnumerable<Color> ColorList
    {
        get => GetValue(ColorListProperty);
        set => SetValue(ColorListProperty, value);
    }
}