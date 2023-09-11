using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.Avalonia.Misc;

public class NamingConventionViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        var viewTypeNameResult = from obj in Maybe.From(data)
            let viewTypeName = obj.GetType().AssemblyQualifiedName.Replace("ViewModel", "View")
            select viewTypeName;

        var view = from viewTypeName in viewTypeNameResult
            from viewType in Maybe.From(Type.GetType(viewTypeName))
            from instance in Maybe.From(Activator.CreateInstance(viewType) as Control)
            select instance;

        return view
            .Or(Fallback(data))
            .GetValueOrThrow();
    }

    public bool Match(object? data)
    {
        return data is IViewModel || (data?.GetType().Name.EndsWith("ViewModel", StringComparison.OrdinalIgnoreCase) ?? false);
    }
    
    private static Control Fallback(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "Data is null: We can't locate any view for null" };
        }

        var inlines = GetInlines(data.GetType().FullName!.Replace("ViewModel", "View"), data);
        var inlineCollection = new InlineCollection();
        inlines.ForEach(inline => inlineCollection.Add(inline));

        return new TextBlock { Inlines = inlineCollection, TextWrapping = TextWrapping.Wrap };
    }

    private static IEnumerable<Inline> GetInlines(string viewTypeName, object data)
    {
        var notFound = new Inline[]
        {
            new Run("View Not Found") { FontWeight = FontWeight.Bold },
        };

        var view = FormatTypeName(viewTypeName);
        var forData = FormatTypeName(data.GetType().FullName);

        return new[] { 
            notFound, 
            Break(),
            Text("Missing view: "), 
            view, 
            Break(),
            Text("While looking for: "), 
            forData, 
            Break(),
            Text($"... as part of Name Convention View Location performed by {nameof(NamingConventionViewLocator)} ") }.SelectMany(x => x);
    }

    private static IEnumerable<Inline> Break()
    {
        yield return new LineBreak();
    }

    private static IEnumerable<Inline> Text(string @string)
    {
        yield return new Run(@string);
    }

    private static IEnumerable<Inline> FormatTypeName(string? viewTypeName)
    {
        if (viewTypeName is null)
        {
            yield return new Run("<null>");
        }

        var chunks = viewTypeName.Split(".");

        for (var i = 0; i < chunks.Length; i++)
        {
            var chunk = chunks[i];

            if (i == chunks.Length - 1)
            {
                yield return new Run(chunk) { FontWeight = FontWeight.Bold };
            }
            else
            {
                yield return new Run(chunk + ".");
            }
        }
    }
}