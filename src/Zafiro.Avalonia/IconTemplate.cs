using Avalonia.Controls.Templates;

namespace Zafiro.Avalonia;

public class IconTemplate : IDataTemplate
{
    public static IconTemplate Instance { get; } = new IconTemplate();
    
    public Control? Build(object? param)
    {
        if (param is not IIcon icon)
            return null;

        if (icon.Source == null)
        {
            return null;
        }

        // 1. División en dos partes: esquema y resto
        var parts = icon.Source.Split(new[] { ':' }, 2);
        if (parts.Length != 2 || parts[0] != "svg")
            return new Projektanker.Icons.Avalonia.Icon() { Value = icon.Source };

        var remainder = parts[1];
        string assemblyName;
        string resourcePath;

        // 2. Formato implícito: /ruta → ensamblado actual
        if (remainder.StartsWith("/"))
        {
            assemblyName = Application.Current!.GetType().Assembly.GetName().Name!;
            resourcePath = remainder.TrimStart('/');
        }
        else
        {
            // 3. Formato explícito: NombreEnsamblado/ruta
            var idx = remainder.IndexOf('/');
            if (idx <= 0)
                return new Projektanker.Icons.Avalonia.Icon { Value = icon.Source }; // formato inválido
                
            assemblyName = remainder[..idx];
            resourcePath = remainder[(idx + 1)..];
        }

        var uri = new Uri($"avares://{assemblyName}");
        return new global::Avalonia.Svg.Svg(uri) { Path = resourcePath};
    }

    public bool Match(object? data) => data is IIcon;
}