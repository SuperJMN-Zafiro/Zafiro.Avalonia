using Icon = Projektanker.Icons.Avalonia.Icon;

namespace Zafiro.Avalonia;

public class IconConverter : IIconConverter
{
    public static IconConverter Instance { get; } = new();

    public Control? Convert(IIcon icon)
    {
        // 1. División en dos partes: esquema y resto
        var parts = icon.Source.Split(new[] { ':' }, 2);
        if (parts.Length != 2 || parts[0] != "svg")
            return new Icon() { Value = icon.Source };

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
                return new Icon { Value = icon.Source }; // formato inválido

            assemblyName = remainder[..idx];
            resourcePath = remainder[(idx + 1)..];
        }

        var uri = new Uri($"avares://{assemblyName}");
        return new global::Avalonia.Svg.Skia.Svg(uri) { Path = resourcePath };
    }
}