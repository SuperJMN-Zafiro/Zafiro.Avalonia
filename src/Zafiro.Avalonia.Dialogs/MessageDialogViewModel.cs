using System.Reactive;
using Zafiro.UI;

namespace Zafiro.Avalonia.Dialogs;

public class MessageDialogViewModel : IResult<Unit>
{
    public string Message { get; }
    private readonly TaskCompletionSource<Unit> tcs = new();

    public MessageDialogViewModel(string message, double width)
    {
        Message = message;
        Width = width;
    }

    public Task<Unit> Result => tcs.Task;
    public double Width { get; }

    public void SetResult(Unit result)
    {
        tcs.SetResult(Unit.Default);
    }
}