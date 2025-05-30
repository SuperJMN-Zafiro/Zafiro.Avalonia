using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.Behaviors;

/// <summary>
/// Sets the current value of an Avalonia property
/// without overriding local bindings or animations.
/// </summary>
[RequiresUnreferencedCode("Incompatible with trimming.")]
public class SetAvaloniaPropertyCurrentValueAction : StyledElementAction
{
    public static readonly StyledProperty<AvaloniaProperty?> TargetPropertyProperty =
        AvaloniaProperty.Register<SetAvaloniaPropertyCurrentValueAction, AvaloniaProperty?>(nameof(TargetProperty));

    public static readonly StyledProperty<AvaloniaObject?> TargetObjectProperty =
        AvaloniaProperty.Register<SetAvaloniaPropertyCurrentValueAction, AvaloniaObject?>(nameof(TargetObject));

    public static readonly StyledProperty<object?> ValueProperty =
        AvaloniaProperty.Register<SetAvaloniaPropertyCurrentValueAction, object?>(nameof(Value));

    public AvaloniaProperty? TargetProperty
    {
        get => GetValue(TargetPropertyProperty);
        set => SetValue(TargetPropertyProperty, value);
    }

    [ResolveByName]
    public AvaloniaObject? TargetObject
    {
        get => GetValue(TargetObjectProperty);
        set => SetValue(TargetObjectProperty, value);
    }

    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    public override object Execute(object? sender, object? parameter)
    {
        if (!IsEnabled)
            return false;

        if (TargetProperty is null)
            throw new ArgumentNullException(nameof(TargetProperty));

        if ((TargetObject ?? sender as AvaloniaObject) is not AvaloniaObject target)
            return false;

        ApplyCurrentValue(target, TargetProperty);
        return true;
    }

    void ApplyCurrentValue(AvaloniaObject target, AvaloniaProperty property)
    {
        if (property.IsReadOnly)
            throw new ArgumentException($"Property {property.Name} is read-only.");

        var converted = ConvertValue(Value, property.PropertyType);
        target.SetCurrentValue(property, converted);
    }

    static object? ConvertValue(object? input, Type targetType)
    {
        if (input is null)
            return targetType.GetTypeInfo().IsValueType
                ? Activator.CreateInstance(targetType)
                : null;

        if (targetType.GetTypeInfo().IsAssignableFrom(input.GetType().GetTypeInfo()))
            return input;

        var text = input.ToString()!;

        if (targetType.GetTypeInfo().IsEnum)
            return Enum.Parse(targetType, text, ignoreCase: true);

        if (typeof(IConvertible).GetTypeInfo().IsAssignableFrom(targetType.GetTypeInfo()))
            return Convert.ChangeType(text, targetType, CultureInfo.InvariantCulture);

        var converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(typeof(string)))
            return converter.ConvertFromInvariantString(text);

        return AvaloniaParseHelper.InvokeParse(text, targetType);
    }
}