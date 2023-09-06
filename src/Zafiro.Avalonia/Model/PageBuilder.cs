using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.Model;

public class PageBuilder<TIn, TOut> where TIn : IValidatable where TOut : IValidatable
{
    private readonly List<IPage> pages;

    private PageBuilder(List<IPage> pages)
    {
        this.pages = pages;
    }

    public PageBuilder(Func<TIn, TOut> factory, List<IPage> pages, Maybe<string> nextText)
    {
        this.pages = pages;
        pages.Add(new Page<TIn, TOut>(factory, nextText));
    }

    public PageBuilder<TOut, TNewOut> WithNext<TNewOut>(Func<TOut, TNewOut> factory, Maybe<string> nextText) where TNewOut : IValidatable
    {
        return new PageBuilder<TOut, TNewOut>(factory, pages, nextText);
    }

    public List<IPage> Build()
    {
        return pages;
    }
}

public static class PageBuilder
{
    public static PageBuilder<Unit, TOut> PageFor<TOut>(Func<TOut> factory, Maybe<string> nextText) where TOut : IValidatable
    {
        return new PageBuilder<Unit, TOut>(_ => factory(), new List<IPage>(), nextText);
    }

    public class Unit : IValidatable
    {
        public IObservable<bool> IsValid { get; }
    }
}