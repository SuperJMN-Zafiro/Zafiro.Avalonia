using Avalonia.Xaml.Interactivity;

namespace Zafiro.Avalonia.Behaviors;

public class EqualityCheckBehavior : Behavior<StyledElement>
{
    public static readonly StyledProperty<object> BindingValueProperty =
        AvaloniaProperty.Register<EqualityCheckBehavior, object>(nameof(BindingValue));
    public object BindingValue
    {
        get => GetValue(BindingValueProperty);
        set => SetValue(BindingValueProperty, value);
    }

    public static readonly StyledProperty<object> CompareValueProperty =
        AvaloniaProperty.Register<EqualityCheckBehavior, object>(nameof(CompareValue));
    public object CompareValue
    {
        get => GetValue(CompareValueProperty);
        set => SetValue(CompareValueProperty, value);
    }
    
    public AvaloniaProperty TargetProperty { get; set; }

    public static readonly StyledProperty<object> TrueValueProperty =
        AvaloniaProperty.Register<EqualityCheckBehavior, object>(nameof(TrueValue));
    public object TrueValue
    {
        get => GetValue(TrueValueProperty);
        set => SetValue(TrueValueProperty, value);
    }

    public static readonly StyledProperty<object> FalseValueProperty =
        AvaloniaProperty.Register<EqualityCheckBehavior, object>(nameof(FalseValue));
    public object FalseValue
    {
        get => GetValue(FalseValueProperty);
        set => SetValue(FalseValueProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        this.GetObservable(BindingValueProperty).Subscribe(_ => Evaluate());
        this.GetObservable(CompareValueProperty).Subscribe(_ => Evaluate());
        Evaluate();
    }
    
    public void Evaluate()
    {
        if (AssociatedObject == null || TargetProperty == null)
            return;

        EqualitySetterHelper.SetValue(
            element: AssociatedObject,
            bindingValue: BindingValue,
            compareValue: CompareValue,
            targetProperty: TargetProperty,
            trueValue: TrueValue,
            falseValue: FalseValue);
    }
}