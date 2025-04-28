using System.Data;
using System.Globalization;

namespace Zafiro.Avalonia.Converters;

public class MathConverters
{
    public static FuncMultiValueConverter<object, double, string> Evaluate { get; } = new((inputs, expression) =>
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        var inputList = inputs.ToList();

        if (inputList.Any(o => o is UnsetValueType))
        {
            return double.NaN;
        }

        var numbers = inputList.Select(Convert.ToDouble).ToList();

        for (int i = 0; i < numbers.Count; i++)
        {
            expression = expression.Replace($"{{{i}}}", numbers[i].ToString(CultureInfo.InvariantCulture));
        }

        return EvaluateExpression(expression);
    });

    private static double EvaluateExpression(string expression)
    {
        var dt = new DataTable();
        return Convert.ToDouble(dt.Compute(expression, ""));
    }
}