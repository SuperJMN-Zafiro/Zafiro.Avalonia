using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Misc;

public class DataTemplateInclude : AvaloniaObject, IDataTemplate
{
    public static readonly StyledProperty<Uri?> SourceProperty = AvaloniaProperty.Register<DataTemplateInclude, Uri?>(
        nameof(Source));

    public DataTemplateInclude(IServiceProvider serviceProvider)
    {
        this
            .WhenAnyValue(x => x.Source)
            .Select(Maybe.From)
            .Select(m =>
            {
                return m.Map(r =>
                {
                    var baseUri = serviceProvider.GetContextBaseUri();
                    return (DataTemplates) AvaloniaXamlLoader.Load(serviceProvider, r, baseUri);
                });
            })
            .BindTo(this, x => x.DataTemplates);
    }

    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Maybe<DataTemplates> DataTemplates { get; set; }

    public Uri? Source
    {
        get => GetValue(SourceProperty);
        set => SetValue(SourceProperty, value);
    }

    public Control? Build(object? param)
    {
        return DataTemplates
            .Bind(templates =>
            {
                return templates
                    .TryFirst(template => template.Match(param))
                    .Bind(template => Maybe.From(template.Build(param)));
            })
            .GetValueOrDefault();
    }

    public bool Match(object? data)
    {
        return DataTemplates.Match(x => x.Any(t => t.Match(data)), () => false);
    }
}