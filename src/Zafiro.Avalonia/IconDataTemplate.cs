using Avalonia.Controls.Templates;

namespace Zafiro.Avalonia;

public class IconDataTemplate : IDataTemplate
{
    public IIconConverter Converter { get; set; } = IconConverter.Instance;

    public Control? Build(object? param)
    {
        if (param is not IIcon icon)
        {
            return null;
        }

        return Converter.Convert(icon);
    }

    public bool Match(object? data) => data is IIcon;
}