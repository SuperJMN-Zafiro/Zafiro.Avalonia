using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.NewWizard.Interfaces;

public interface IPage
{
    void UpdateWith(object input);
    object Content { get; }
}

public interface IPage<TIn, TOut>
{
    void UpdateWith(TIn input);
    TOut Content { get; }
}