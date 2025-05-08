using System.Globalization;
using System.Text;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace Zafiro.Avalonia.Misc;

public class NameCollapsingProperties : TextCollapsingProperties
{
    public NameCollapsingProperties(double width, FlowDirection flowDirection)
    {
        Width = width;
        FlowDirection = flowDirection;
        Symbol = null;
    }

    public override double Width { get; }
    public override TextRun Symbol { get; }
    public override FlowDirection FlowDirection { get; }

    public override TextRun[]? Collapse(TextLine textLine)
    {
        var text = GetTextFromTextLine(textLine);
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        var options = GenerateNameOptions(text);

        var textRunProperties = textLine.TextRuns.First().Properties;
        var culture = textRunProperties.CultureInfo ?? CultureInfo.CurrentCulture;

        ShapedTextRun? shortestOptionRun = null;

        // Retrieve the IGlyphTypeface
        var glyphTypeface = textRunProperties.Typeface.GlyphTypeface;
        if (glyphTypeface == null)
        {
            return null; // Handle the case where glyphTypeface is null
        }

        // Determine the bidiLevel based on FlowDirection
        sbyte bidiLevel = (sbyte)(FlowDirection == FlowDirection.RightToLeft ? 1 : 0);

        foreach (var option in options)
        {
            // Enhance TextShaperOptions
            var textShaperOptions = new TextShaperOptions(
                glyphTypeface,
                textRunProperties.FontRenderingEmSize,
                bidiLevel,
                culture);

            // Use TextShaper to shape the text
            var textShaper = TextShaper.Current;

            var shapedBuffer = textShaper.ShapeText(
                option.AsMemory(),
                textShaperOptions);

            // Enhance the ShapedTextRun
            var shapedTextRun = new ShapedTextRun(
                shapedBuffer,
                textRunProperties);

            // Measure the ShapedTextRun width
            var width = shapedTextRun.Size.Width;

            if (width <= Width)
            {
                return [shapedTextRun];
            }

            // Store the shortest option
            if (shortestOptionRun == null || option.Length < shortestOptionRun.ShapedBuffer.Length)
            {
                shortestOptionRun = shapedTextRun;
            }
        }

        // If no option fits, use standard truncation
        var defaultCollapsingProperties = TextTrimming.CharacterEllipsis.CreateCollapsingProperties(new TextCollapsingCreateInfo(Width, textRunProperties, FlowDirection));
        return defaultCollapsingProperties.Collapse(textLine);
    }

    private string GetTextFromTextLine(TextLine textLine)
    {
        var stringBuilder = new StringBuilder();

        foreach (var run in textLine.TextRuns)
        {
            if (run is ShapedTextRun shapedRun)
            {
                var text = shapedRun.ShapedBuffer.Text;
                stringBuilder.Append(text);
            }
            else if (run is TextCharacters textCharacters)
            {
                stringBuilder.Append(textCharacters.Text);
            }
        }

        return stringBuilder.ToString();
    }

    private IEnumerable<string> GenerateNameOptions(string fullName)
    {
        var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var options = new List<string>();

        // Option 1: Full name
        options.Add(fullName);

        if (names.Length >= 2)
        {
            // Option 2: Initials of first names, full last name (e.g., "J.M. Nieto")
            var initials = string.Join(".", names.Take(names.Length - 1).Select(n => n[0])) + ".";
            var lastName = names.Last();
            options.Add($"{initials} {lastName}");

            // Option 3: First full name, initials of middle names, full last name
            if (names.Length >= 3)
            {
                var firstName = names[0];
                var middleInitials = string.Join(".", names.Skip(1).Take(names.Length - 2).Select(n => n[0])) + ".";
                options.Add($"{firstName} {middleInitials} {lastName}");
            }

            // Option 4: Only initials (e.g., "J.M.N.")
            var allInitials = string.Join(".", names.Select(n => n[0])) + ".";
            options.Add(allInitials);
        }

        return options;
    }
}