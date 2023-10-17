using System.Threading.Tasks;
using Zafiro;
using Zafiro.UI;

namespace TestApp.Samples.Dialogs;

public class MyViewModel : IResult<string>
{
    private TaskCompletionSource<string> tcs = new();
    public Task<string> Result => tcs.Task;
    public string Text { get; set; }

    public void SetResult(string value)
    {
        tcs.SetResult(value);
    }
}