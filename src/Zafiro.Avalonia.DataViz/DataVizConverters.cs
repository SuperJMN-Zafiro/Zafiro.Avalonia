using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using MoreLinq;
using Zafiro.DataAnalysis.Clustering;

namespace Zafiro.Avalonia.DataViz;

public class DataVizConverters
{
    public static readonly FuncMultiValueConverter<object, bool> AreEquals =
        new(
            objects =>
            {
                var enumerable = objects as object[] ?? objects.ToArray();
                var a = enumerable.First();
                var b = enumerable.Last();

                if (a?.GetType() == typeof(UnsetValueType))
                {
                    return false;
                }

                if (b?.GetType() == typeof(UnsetValueType))
                {
                    return false;
                }

                var areEqual = a != null && a.Equals(b);

                return areEqual;
            });
    
    public static FuncValueConverter<object, bool> ContentVisibilityConverter = new FuncValueConverter<object, bool>(o =>
    {
        if (o == null)
        {
            return false;
        }

        if (o is ClusterNode c)
        {
            if (c.Item is null)
            {
                return false;
            }
        }

        return true;
    });

    public static FuncMultiValueConverter<double, double> Multiply = new FuncMultiValueConverter<double, double>(doubles =>
    {
        //var a = doubles.ToList()[0];
        //var b = doubles.ToList()[1];
        //return a * b;

        return doubles.Aggregate((a, b) => a * b);
    });

    public static FuncMultiValueConverter<object, double> Divide = new FuncMultiValueConverter<object, double>(objects =>
    {
        //var a = doubles.ToList()[0];
        //var b = doubles.ToList()[1];
        //return a * b;
        
        if (objects.Any(o => o is UnsetValueType))
        {
            return 1;
        };

        return objects.Select(o => Convert.ToDouble(o)).Aggregate((a, b) => a / b);
    });

    public static FuncValueConverter<IEnumerable<Color>, GradientStops> ColorsToGradientStops =
        new FuncValueConverter<IEnumerable<Color>, GradientStops>(colors =>
        {
            if (colors == null)
            {
                return new GradientStops();
            }

            var colorList = colors.ToList();
            var totalColors  = colorList.Count();
            var gradientStops = new GradientStops();

            var step = (double)1 / (totalColors-1);

            colorList.Select((color, i) => (Color: color, Offset: step * i)).ForEach(color => gradientStops.Add(new GradientStop(color.Color, color.Offset)));
            return gradientStops;
        });
}

