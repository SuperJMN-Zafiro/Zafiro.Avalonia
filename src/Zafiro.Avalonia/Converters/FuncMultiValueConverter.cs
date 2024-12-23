using System.Globalization;
using Avalonia.Data.Converters;

namespace Zafiro.Avalonia.Converters;

public class FuncMultiValueConverter<TIn, TOut, TParameter> : IMultiValueConverter
{
    private readonly Func<IEnumerable<TIn?>, TParameter?, TOut> convert;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuncMultiValueConverter{TIn, TOut, TParameter}"/> class.
    /// </summary>
    /// <param name="convert">The convert function that takes the input values and a parameter.</param>
    public FuncMultiValueConverter(Func<IEnumerable<TIn?>, TParameter?, TOut> convert)
    {
        this.convert = convert;
    }

    /// <inheritdoc/>
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        //standard OfType skip null values, even they are valid for the Type
        static IEnumerable<TIn?> OfTypeWithDefaultSupport(IList<object?> list)
        {
            foreach (var obj in list)
            {
                if (obj is TIn result)
                {
                    yield return result;
                }
                else if (Equals(obj, default(TIn)))
                {
                    yield return default;
                }
            }
        }

        var converted = OfTypeWithDefaultSupport(values).ToList();

        if (converted.Count == values.Count)
        {
            var typedParameter = parameter is TParameter tp ? tp : default;
            return convert(converted, typedParameter);
        }
        else
        {
            return AvaloniaProperty.UnsetValue;
        }
    }
}