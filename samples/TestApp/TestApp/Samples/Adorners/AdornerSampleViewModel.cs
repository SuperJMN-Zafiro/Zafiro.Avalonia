using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace TestApp.Samples.Adorners;

[Icon("fa-cog")]
public class AdornerSampleViewModel : ReactiveObject
{
    public AdornerSampleViewModel()
    {
        LengthyCommand = ReactiveCommand.CreateFromTask(() => Task.Delay(1000));
        IsExecuting = LengthyCommand.IsExecuting;
    }

    public IObservable<bool> IsExecuting { get; set; }

    public ReactiveCommand<Unit, Unit> LengthyCommand { get; set; }
}

public class IconAttribute(string id) : Attribute
{
    public string Id { get; } = id;
}