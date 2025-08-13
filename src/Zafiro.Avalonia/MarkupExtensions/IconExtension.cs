using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace Zafiro.Avalonia.MarkupExtensions;

/// <summary>
/// Markup extension that simplifies the use of icons in XAML.
/// Usage: {Icon fa-wallet} or {Icon Source=fa-wallet}
/// </summary>
public class IconExtension : MarkupExtension
{
    /// <summary>
    /// Constructor for positional parameter usage
    /// </summary>
    public IconExtension()
    {
    }

    /// <summary>
    /// Constructor that accepts the icon source as a positional parameter
    /// </summary>
    /// <param name="source">The icon source string</param>
    public IconExtension(string source)
    {
        Source = source;
    }

    /// <summary>
    /// The icon source string (e.g., "fa-wallet" for FontAwesome icons)
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Provides the Icon instance when the markup extension is evaluated
    /// </summary>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Source))
        {
            return BindingOperations.DoNothing;
        }

        return new Icon(Source);
    }
}