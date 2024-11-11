using System.Globalization;
using System.Text;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace Zafiro.Avalonia.Misc;

public class NameCollapsingProperties : TextCollapsingProperties
{
    public override double Width { get; }
    public override TextRun Symbol { get; }
    public override FlowDirection FlowDirection { get; }

    public NameCollapsingProperties(double width, FlowDirection flowDirection)
    {
        Width = width;
        FlowDirection = flowDirection;
        Symbol = null;
    }

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

        var glyphTypeface = textRunProperties.Typeface.GlyphTypeface;

        sbyte bidiLevel = (sbyte)(FlowDirection == FlowDirection.RightToLeft ? 1 : 0);

        foreach (var option in options)
        {
            var textShaperOptions = new TextShaperOptions(
                glyphTypeface,
                textRunProperties.FontRenderingEmSize,
                bidiLevel,
                culture);

            var textShaper = TextShaper.Current;

            var shapedBuffer = textShaper.ShapeText(
                option.AsMemory(),
                textShaperOptions);

            var shapedTextRun = new ShapedTextRun(
                shapedBuffer,
                textRunProperties);

            var width = shapedTextRun.Size.Width;

            if (width <= Width)
            {
                return [shapedTextRun];
            }

            if (shortestOptionRun == null || option.Length < shortestOptionRun.ShapedBuffer.Length)
            {
                shortestOptionRun = shapedTextRun;
            }
        }

        return [shortestOptionRun!];
    }


    private static string GetTextFromTextLine(TextLine textLine)
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

    private static IEnumerable<string> GenerateNameOptions(string fullName)
    {
        var names = fullName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var options = new List<string>();

        // Option 1: Full name
        options.Add(fullName);

        // Option 2: Initials of first names, full last name (e.g., "J.M. Nieto")
        if (names.Length >= 2)
        {
            var initials = string.Join(".", names.Take(names.Length - 1).Select(n => n[0])) + ".";
            var lastName = names.Last();
            options.Add($"{initials} {lastName}");
        }

        // Option 3: Full first name, initial of the middle name, full last name (e.g., "José M. Nieto")
        if (names.Length >= 3)
        {
            var firstName = names[0];
            var middleInitials = string.Join(".", names.Skip(1).Take(names.Length - 2).Select(n => n[0])) + ".";
            var lastName = names.Last();
            options.Add($"{firstName} {middleInitials} {lastName}");
        }

        // Option 4: Initials only (e.g., "J.M.N.")
        var allInitials = string.Join(".", names.Select(n => n[0])) + ".";
        options.Add(allInitials);

        return options;
    }
}