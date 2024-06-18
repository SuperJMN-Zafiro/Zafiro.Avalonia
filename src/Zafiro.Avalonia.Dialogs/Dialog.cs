using Avalonia;
using Avalonia.Controls.Templates;

namespace Zafiro.Avalonia.Dialogs;

public class Dialog
{
    public static readonly AttachedProperty<DataTemplates> TemplatesProperty =
        AvaloniaProperty.RegisterAttached<Dialog, Application, DataTemplates>("Templates", new DataTemplates());

    public static void SetTemplates(Application obj, DataTemplates value) => obj.SetValue(TemplatesProperty, value);
    public static DataTemplates GetTemplates(Application obj) => obj.GetValue(TemplatesProperty);
}