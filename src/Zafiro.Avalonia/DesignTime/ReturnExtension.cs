using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Avalonia;
using Avalonia.Data.Core;
using Avalonia.Markup.Xaml;

namespace Zafiro.Avalonia.DesignTime;

public class ReturnExtension : MarkupExtension
{
    public ReturnExtension()
    {
    }

    public ReturnExtension(object value)
    {
        Value = value;
    }

    public override object? ProvideValue(IServiceProvider serviceProvider)
    {
        if (Value is null)
        {
            return AvaloniaProperty.UnsetValue;
        }

        var service = (IProvideValueTarget)serviceProvider.GetService(typeof(IProvideValueTarget))!;
        var type = ((ClrPropertyInfo) service.TargetProperty).PropertyType.GenericTypeArguments[0];
        Type observableType = typeof(System.Reactive.Linq.Observable);
        MethodInfo returnMethodInfo = observableType.GetMethods().First(x => x.Name == "Return" && x.GetParameters().Length == 1);
        var method = returnMethodInfo.MakeGenericMethod(type);

        var finalValue = ProvideValue(type);

        var provideValue = method.Invoke(null, new[] { finalValue });
        return provideValue;
    }

    private object? ProvideValue(Type targetType)
    {
        object finalValue;
        if (targetType.IsAssignableFrom(this.Value.GetType()))
        {
            finalValue = Value;
        }
        else
        {
            var tc = TypeDescriptor.GetConverter(targetType);
            finalValue = tc.ConvertFrom(null, CultureInfo.InvariantCulture, Value);
        }

        return finalValue;
    }

    public object? Value { get; set; }
}