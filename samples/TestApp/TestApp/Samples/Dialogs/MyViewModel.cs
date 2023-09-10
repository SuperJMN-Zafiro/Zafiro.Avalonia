using System.Threading.Tasks;
using Zafiro.Avalonia.Dialogs;
using Zafiro.Avalonia.Misc;

namespace TestApp.Samples.Dialogs;

public class MyViewModel : IHaveResult<string>, IViewModel
{
    private TaskCompletionSource<string> tcs = new();
    public Task<string> Result => tcs.Task;
    public string Text { get; set; }

    public void SetResult(string value)
    {
        tcs.SetResult(value);
    }
}