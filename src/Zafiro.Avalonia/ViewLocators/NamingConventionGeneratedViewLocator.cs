using Avalonia.Controls.Documents;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.Avalonia.ViewLocators;

// Generated naming-convention view locator (VM->View, same namespace), AOT-friendly via global registration
public class NamingConventionGeneratedViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Func<Control>> GlobalRegistry = new();

    public Control Build(object? data)
    {
        var view = TryFromRegistry(data);
        return view.Or(Fallback(data)).GetValueOrThrow();
    }

    public bool Match(object? data)
    {
        if (data is null)
        {
            return false;
        }

        return GlobalRegistry.ContainsKey(data.GetType());
    }

    public static void RegisterGlobal(Type viewModelType, Func<Control> factory)
    {
        GlobalRegistry[viewModelType] = factory;
    }

    public static void RegisterGlobal<TViewModel, TView>() where TView : Control, new()
    {
        RegisterGlobal(typeof(TViewModel), () => new TView());
    }

    private static Maybe<Control> TryFromRegistry(object? data)
    {
        return Maybe.From(data)
            .Bind(d => GlobalRegistry.TryGetValue(d.GetType(), out var factory)
                ? Maybe.From(factory())
                : Maybe<Control>.None);
    }

    private static Control Fallback(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "Data is null: We can't locate any view for null" };
        }

        var vmType = data.GetType();
        var viewTypeName = vmType.FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);

        var inlines = GetInlines(viewTypeName, data);
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

        return new[]
        {
            notFound,
            Break(),
            Text("Missing view: "),
            view,
            Break(),
            Text("While looking for: "),
            forData,
            Break(),
            Text($"... as part of Name Convention View Location performed by {nameof(NamingConventionGeneratedViewLocator)} ")
        }.SelectMany(x => x);
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