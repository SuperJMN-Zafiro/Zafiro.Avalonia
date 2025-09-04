using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Zafiro.Avalonia.Misc;

namespace Zafiro.Avalonia.DataViz.Heatmaps;

public class HeatmapConverter
{
    public static readonly FuncMultiValueConverter<object, IBrush> Instance =
        new(objects =>
        {
            var list = objects.ToList();
            if (list.Any(o => o is UnsetValueType))
            {
                return Brushes.Black;
            }

            var colorList = ((IEnumerable<Color>)list[0]).ToList();
            var value = (double)list[1];

            return new SolidColorBrush(ColorInterpolator.InterpolateColor(colorList.ToList(), value));
        });
}