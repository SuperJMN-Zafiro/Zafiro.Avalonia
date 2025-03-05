using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.Behaviors;

public class VisibleWhenBehavior : Behavior<Control>
{
    public static readonly StyledProperty<object> BindingValueProperty =
        AvaloniaProperty.Register<VisibleWhenBehavior, object>(nameof(BindingValue));
    public object BindingValue
    {
        get => GetValue(BindingValueProperty);
        set => SetValue(BindingValueProperty, value);
    }

    public static readonly StyledProperty<object> CompareValueProperty =
        AvaloniaProperty.Register<VisibleWhenBehavior, object>(nameof(CompareValue));
    public object CompareValue
    {
        get => GetValue(CompareValueProperty);
        set => SetValue(CompareValueProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        this.GetObservable(BindingValueProperty).Subscribe(_ => Evaluate());
        this.GetObservable(CompareValueProperty).Subscribe(_ => Evaluate());
        Evaluate();
    }

    private void Evaluate()
    {
        if (AssociatedObject == null)
            return;

        EqualitySetterHelper.SetValue(
            element: AssociatedObject,
            bindingValue: BindingValue,
            compareValue: CompareValue,
            targetProperty: Visual.IsVisibleProperty,
            trueValue: true,
            falseValue: false);
    }
}