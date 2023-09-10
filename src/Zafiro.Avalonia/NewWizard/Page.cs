using Zafiro.Avalonia.NewWizard.Interfaces;

namespace Zafiro.Avalonia.NewWizard;

public class Page<TIn, TOut> : IPage<TIn, TOut>, IPage
{
    private readonly Func<TIn, TOut> factory;

    public Page(Func<TIn, TOut> factory, string nextText)
    {
        this.factory = factory;
        NextText = nextText;
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
    public string NextText { get; }
    public TOut Content { get; private set; }
}