namespace Zafiro.Avalonia.WizardOld.Interfaces;

public interface IPage
{
    void UpdateWith(object input);
    object Content { get; }
    string NextText { get; }
}

public interface IPage<TIn, TOut>
{
    void UpdateWith(TIn input);
    TOut Content { get; }
    string NextText { get; }
}