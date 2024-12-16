namespace Zafiro.Avalonia.Controls.Wizards.Builder;

public static class WizardBuilder
{
    public static WizardBuilder<T> StartWith<T>(Func<T> start) where T : IValidatable
    {
        return new WizardBuilder<T>(start);
    }
}

public class WizardBuilder<TCurrent> where TCurrent : IValidatable
{
    private readonly List<Func<IValidatable?, IValidatable>> steps;

    internal WizardBuilder(Func<TCurrent> start)
    {
        steps = new List<Func<IValidatable?, IValidatable>> { _ => start() };
    }

    private WizardBuilder(List<Func<IValidatable?, IValidatable>> steps)
    {
        this.steps = steps;
    }

    public WizardBuilder<TNext> Then<TNext>(Func<TCurrent, TNext> factory) where TNext : IValidatable
    {
        steps.Add(prev => factory((TCurrent)prev!));
        return new WizardBuilder<TNext>(steps);
    }

    public List<Func<IValidatable?, IValidatable>> Build()
    {
        return steps;
    }
}