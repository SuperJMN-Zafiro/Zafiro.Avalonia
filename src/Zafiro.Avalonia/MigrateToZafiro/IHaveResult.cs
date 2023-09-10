namespace Zafiro.Avalonia.MigrateToZafiro;

public interface IHaveResult<T>
{
    Task<T> Result { get; }
    void SetResult(T result);
}