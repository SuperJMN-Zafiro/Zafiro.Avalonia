using Avalonia.Controls.Documents;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.Avalonia.ViewLocators;

// View locator that uses global registrations generated from x:DataType (including interfaces)
public class DataTypeViewLocator : IDataTemplate
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

        var type = data.GetType();
        if (GlobalRegistry.ContainsKey(type))
        {
            return true;
        }

        foreach (var @interface in type.GetInterfaces())
        {
            if (GlobalRegistry.ContainsKey(@interface))
            {
                return true;
            }
        }

        return false;
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
        return Maybe.From(data).Bind(d =>
        {
            var type = d.GetType();

            // 1) Exact type match
            if (GlobalRegistry.TryGetValue(type, out var factory))
            {
                return Maybe.From(factory());
            }

            // 2) Match by any implemented interface
            foreach (var @interface in type.GetInterfaces())
            {
                if (GlobalRegistry.TryGetValue(@interface, out var ifFactory))
                {
                    return Maybe.From(ifFactory());
                }
            }

            return Maybe<Control>.None;
        });
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
            Text($"... as part of DataType-based View Location performed by {nameof(DataTypeViewLocator)} ")
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