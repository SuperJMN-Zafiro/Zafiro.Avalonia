using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Zafiro.Avalonia.Controls
{
    public class Info : TemplatedControl
    {
        public static readonly StyledProperty<object> IconContentProperty = AvaloniaProperty.Register<Info, object>(nameof(IconContent));

        public object IconContent
        {
            get => GetValue(IconContentProperty);
            set => SetValue(IconContentProperty, value);
        }

        public static readonly StyledProperty<object> ToolTipContentProperty = AvaloniaProperty.Register<Info, object>(nameof(ToolTipContent));

        public object ToolTipContent
        {
            get => GetValue(ToolTipContentProperty);
            set => SetValue(ToolTipContentProperty, value);
        }
    }
}
