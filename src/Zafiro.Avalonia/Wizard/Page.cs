using Zafiro.Avalonia.Wizard.Interfaces;

namespace Zafiro.Avalonia.Wizard;

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
        var i = (TIn) input;
        UpdateWith(i);
    }

    object IPage.Content => Content;
    public string NextText { get; }
    public TOut Content { get; private set; }
}