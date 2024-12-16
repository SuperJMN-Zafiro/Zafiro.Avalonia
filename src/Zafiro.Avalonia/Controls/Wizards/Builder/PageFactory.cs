namespace Zafiro.Avalonia.Controls.Wizards.Builder;

internal class PageFactory
{
    private readonly Func<IValidatable> factory;
    private IValidatable? instance;

    public PageFactory(Func<IValidatable> factory)
    {
        this.factory = factory;
    }

    public IValidatable GetInstance()
    {
        return instance ??= factory();
    }
}