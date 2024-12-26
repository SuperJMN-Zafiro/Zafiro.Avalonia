using System.Reactive.Disposables;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using Zafiro.Avalonia.MigrateToZafiro;

namespace Zafiro.Avalonia.Behaviors;

public class UntouchedClassBehavior : Behavior<Control>
{
    private bool hasBeenModified;
    private readonly CompositeDisposable disposables = new();

    public static readonly StyledProperty<string> ClassNameProperty = AvaloniaProperty.Register<UntouchedClassBehavior, string>(
        nameof(ClassName), "untouched");

    public string ClassName
    {
        get => GetValue(ClassNameProperty);
        set => SetValue(ClassNameProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        
        if (AssociatedObject != null)
        {
            AssociatedObject.Classes.Add(ClassName);

            if (AssociatedObject is IModifiable modifiable)
            {
                HandleModifiable(modifiable);
            }
            else
            {
                DefaultHandle();
            }
        }
    }

    private void HandleModifiable(IModifiable modifiable)
    {
        if (AssociatedObject == null)
        {
            return;
        }
        
        modifiable.Modified.Do(_ => AssociatedObject.Classes.Remove(ClassName)).Subscribe().DisposeWith(disposables);
    }

    private void DefaultHandle()
    {
        if (AssociatedObject == null)
        {
            return;
        }
        
        var textProperty = AssociatedObject switch
        {
            NumericUpDown => NumericUpDown.TextProperty,
            TextBox => TextBox.TextProperty,
            _ => null
        };

        if (textProperty != null)
        {
            AssociatedObject.GetObservable(textProperty)
                .Subscribe(_ => OnTextChanged())
                .DisposeWith(disposables);
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        disposables.Dispose();
        if (AssociatedObject != null)
        {
            if (AssociatedObject.GetVisualRoot() is {})
            {
                AssociatedObject.Classes.Remove(ClassName);
            }
        }
    }

    private void OnTextChanged()
    {
        if (!hasBeenModified && AssociatedObject?.GetVisualRoot() is {})
        {
            hasBeenModified = true;
            AssociatedObject.Classes.Remove(ClassName);
        }
    }
}