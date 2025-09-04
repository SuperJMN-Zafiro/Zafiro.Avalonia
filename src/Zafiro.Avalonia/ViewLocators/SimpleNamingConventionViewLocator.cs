using Avalonia.Controls.Templates;

namespace Zafiro.Avalonia.ViewLocators;

// Simple view locator by naming convention: Namespace.SomethingViewModel => Namespace.SomethingView
public class SimpleNamingConventionViewLocator : IDataTemplate
{
    public Control? Build(object? data)
    {
        if (data is null)
            return null;

        var vmType = data.GetType();
        var viewTypeName = vmType.FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);

        // Try same assembly first
        var type = vmType.Assembly.GetType(viewTypeName)
                   ?? AppDomain.CurrentDomain.GetAssemblies()
                       .Select(a => a.GetType(viewTypeName))
                       .FirstOrDefault(t => t is not null);

        if (type is not null && typeof(Control).IsAssignableFrom(type))
        {
            var control = (Control)Activator.CreateInstance(type)!;
            control.DataContext = data;
            return control;
        }

        return new TextBlock { Text = "Not Found: " + viewTypeName };
    }

    public bool Match(object? data)
    {
        if (data is null)
            return false;

        var vmType = data.GetType();
        var viewTypeName = vmType.FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var type = vmType.Assembly.GetType(viewTypeName)
                   ?? AppDomain.CurrentDomain.GetAssemblies()
                       .Select(a => a.GetType(viewTypeName))
                       .FirstOrDefault(t => t is not null);

        return type is not null && typeof(Control).IsAssignableFrom(type);
    }
}