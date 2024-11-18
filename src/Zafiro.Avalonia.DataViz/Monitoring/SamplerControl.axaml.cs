using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.DataViz.Monitoring;

public class SamplerControl : TemplatedControl
{
    public static readonly StyledProperty<IEnumerable<double>> ValuesProperty = AvaloniaProperty.Register<SamplerControl, IEnumerable<double>>(
        nameof(Values));

    public IEnumerable<double> Values
    {
        get => GetValue(ValuesProperty);
        set => SetValue(ValuesProperty, value);
    }

    public static readonly StyledProperty<double> VerticalStepIntervalProperty = AvaloniaProperty.Register<SamplerControl, double>(
        nameof(VerticalStepInterval), 50d);

    public double VerticalStepInterval
    {
        get => GetValue(VerticalStepIntervalProperty);
        set => SetValue(VerticalStepIntervalProperty, value);
    }

    public static readonly StyledProperty<double> XStepProperty = AvaloniaProperty.Register<SamplerControl, double>(
        nameof(XStep), 10d);

    public double XStep
    {
        get => GetValue(XStepProperty);
        set => SetValue(XStepProperty, value);
    }
}