namespace Zafiro.Avalonia.Controls.Wizards.Builder;

public static class WizardBuilder
{
    public static WizardBuilder<TFirst> StartWith<TFirst>(Func<TFirst> start) where TFirst : IStep
    {
        return new WizardBuilder<TFirst>(start);
    }
}

public class WizardBuilder<TCurrent> where TCurrent : IStep
{
    private readonly List<Func<IStep?, IStep>> steps;

    internal WizardBuilder(Func<TCurrent> start)
    {
        steps = new List<Func<IStep?, IStep>> { _ => start() };
    }

    private WizardBuilder(List<Func<IStep?, IStep>> steps)
    {
        this.steps = steps;
    }

    public WizardBuilder<TNext> Then<TNext>(Func<TCurrent, TNext> factory) where TNext : IStep
    {
        steps.Add(prev => factory((TCurrent)prev!));
        return new WizardBuilder<TNext>(steps);
    }

    public IWizard<TResult> FinishWith<TResult>(Func<TCurrent, TResult> resultFactory)
    {
        return new Wizard<TResult>(
            steps, 
            last => resultFactory((TCurrent)last)
        );
    }
}