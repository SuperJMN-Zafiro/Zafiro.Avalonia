using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Model;

public class Page<TIn, TOut> : IPage where TIn : IValidatable where TOut : IValidatable
{
    private readonly Func<TIn?, TOut> factory;

    public Page(Func<TIn?, TOut> factory, Maybe<string> nextText)
    {
        NextText = nextText;
        this.factory = factory;
    }

    private TOut Create(TIn? input)
    {
        return factory(input);
    }

    public IValidatable? Content { get; private set; }

    public void CreateContent(object? param)
    {
        Content = Create((TIn?)param);
    }

    public Maybe<string> NextText { get;  }
}