using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Zafiro.Avalonia.DataViz.Monitoring;

public class SamplerControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<double>> ValuesProperty = AvaloniaProperty.Register<SamplerControl, IEnumerable<double>>(
        nameof(Values));

    public static readonly StyledProperty<double> VerticalStepIntervalProperty = AvaloniaProperty.Register<SamplerControl, double>(
        nameof(VerticalStepInterval), 50d);

    public static readonly StyledProperty<double> XStepProperty = AvaloniaProperty.Register<SamplerControl, double>(
        nameof(XStep), 10d);

    public static readonly StyledProperty<IBrush> PlotBrushProperty = AvaloniaProperty.Register<SamplerControl, IBrush>(
        nameof(PlotBrush));

    public static readonly StyledProperty<IBrush> ZeroHorizontalLineBrushProperty = AvaloniaProperty.Register<SamplerControl, IBrush>(
        nameof(ZeroHorizontalLineBrush));

    public static readonly StyledProperty<IBrush> NonZeroHorizontalLineBrushProperty = AvaloniaProperty.Register<SamplerControl, IBrush>(
        nameof(NonZeroHorizontalLineBrush));

    public IEnumerable<double> Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public double VerticalStepInterval
    {
        get => GetValue(VerticalStepIntervalProperty);
        set => SetValue(VerticalStepIntervalProperty, value);
    }

    public double XStep
    {
        get => GetValue(XStepProperty);
        set => SetValue(XStepProperty, value);
    }

    public IBrush PlotBrush
    {
        get => GetValue(PlotBrushProperty);
        set => SetValue(PlotBrushProperty, value);
    }

    public IBrush ZeroHorizontalLineBrush
    {
        get => GetValue(ZeroHorizontalLineBrushProperty);
        set => SetValue(ZeroHorizontalLineBrushProperty, value);
    }

    public IBrush NonZeroHorizontalLineBrush
    {
        get => GetValue(NonZeroHorizontalLineBrushProperty);
        set => SetValue(NonZeroHorizontalLineBrushProperty, value);
    }
}