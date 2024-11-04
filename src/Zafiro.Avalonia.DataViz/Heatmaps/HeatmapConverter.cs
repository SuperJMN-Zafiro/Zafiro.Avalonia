using System.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Heatmaps;

public class HeatmapConverter
{
    public static readonly FuncMultiValueConverter<object, IBrush> Instance =
        new(
            objects =>
            {
                var list = objects.ToList();
                if (list.Any(o => o is UnsetValueType))
                {
                    return Brushes.Black;
                }

                var lowBrush = (Color)list[0];
                var highBrush = (Color)list[1];
                var value = (double)list[2];

                // Interpolación lineal para cada componente de color
                var r = (byte)(lowBrush.R + (highBrush.R - lowBrush.R) * value);
                var g = (byte)(lowBrush.G + (highBrush.G - lowBrush.G) * value);
                var b = (byte)(lowBrush.B + (highBrush.B - lowBrush.B) * value);

                return new SolidColorBrush(Color.FromRgb(r, g, b));
            });
}