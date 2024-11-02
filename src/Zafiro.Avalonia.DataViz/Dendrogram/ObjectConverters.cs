using System.Linq;
using Avalonia;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.DataViz.Dendrogram;

public class ObjectConverters
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
            if (c.Content is null)
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
}

