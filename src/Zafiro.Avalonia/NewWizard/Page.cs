using Zafiro.Avalonia.NewWizard.Interfaces;

namespace Zafiro.Avalonia.NewWizard;

public class Page<TIn, TOut> : IPage<TIn, TOut>, IPage
{
    private readonly Func<TIn, TOut> factory;

    public Page(Func<TIn, TOut> factory)
    {
        this.factory = factory;
    }

    public void UpdateWith(TIn input)
    {
        Content = factory(input);
    }

    public void UpdateWith(object input)
    {
        UpdateWith((TIn) input);
    }

    object IPage.Content => Content;
    public TOut Content { get; private set; }
}