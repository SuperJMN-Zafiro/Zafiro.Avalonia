namespace Zafiro.Avalonia.Controls;

public class StoppableControl : ContentControl
{
    public static readonly StyledProperty<IStoppableCommand> CommandProperty = AvaloniaProperty.Register<StoppableControl, IStoppableCommand>(nameof(Command));

    public StoppableControl()
    {
        StopContent = "Stop";
    }

    public IStoppableCommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object> StopContentProperty = AvaloniaProperty.Register<StoppableControl, object>(
        nameof(StopContent));

    public object StopContent
    {
        get => GetValue(StopContentProperty);
        set => SetValue(StopContentProperty, value);
    }
}