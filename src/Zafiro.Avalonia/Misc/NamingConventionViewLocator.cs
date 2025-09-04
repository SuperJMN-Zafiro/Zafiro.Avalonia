using Avalonia.Controls.Documents;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using CSharpFunctionalExtensions;
using MoreLinq;

namespace Zafiro.Avalonia.Misc;

public partial class NamingConventionViewLocator : IDataTemplate
{
    private static readonly Dictionary<Type, Func<Control>> GlobalRegistry = new();
    private readonly Dictionary<Type, Func<Control>> registry = new();

    public NamingConventionViewLocator()
    {
        // Keep legacy partial-registration (only populated within the same assembly)
        AutoRegister();

        // Merge global registrations (populated via ModuleInitializer from any assembly)
        foreach (var kv in GlobalRegistry)
        {
            registry[kv.Key] = kv.Value;
        }
    }

    public Control Build(object? data)
    {
        var view = TryFromRegistry(data);

        return view
            .Or(Fallback(data))
            .GetValueOrThrow();
    }

    public bool Match(object? data)
    {
        return data is IViewModel || (data?.GetType().Name.Contains("ViewModel", StringComparison.OrdinalIgnoreCase) ?? false);
    }

    // Legacy: implemented by generator in Zafiro.Avalonia project only
    partial void AutoRegister();

    // Instance-level registration API (fluent)
    public NamingConventionViewLocator Register<TViewModel, TView>()
        where TView : Control, new()
    {
        registry[typeof(TViewModel)] = () => new TView();
        return this;
    }

    public NamingConventionViewLocator Register<TViewModel>(Func<Control> factory)
    {
        registry[typeof(TViewModel)] = factory;
        return this;
    }

    // Global registration API used by source generator in any assembly
    public static void RegisterGlobal(Type viewModelType, Func<Control> factory)
    {
        GlobalRegistry[viewModelType] = factory;
    }

    public static void RegisterGlobal<TViewModel, TView>() where TView : Control, new()
    {
        RegisterGlobal(typeof(TViewModel), () => new TView());
    }

    private Maybe<Control> TryFromRegistry(object? data)
    {
        return Maybe.From(data)
            .Bind(d =>
            {
                var type = d.GetType();

                // 1) Exact type match
                if (registry.TryGetValue(type, out var factory))
                {
                    return Maybe.From(factory());
                }

                // 2) Match by any implemented interface
                foreach (var @interface in type.GetInterfaces())
                {
                    if (registry.TryGetValue(@interface, out var ifFactory))
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
            Text($"... as part of Name Convention View Location performed by {nameof(NamingConventionViewLocator)} ")
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