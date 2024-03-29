﻿using System;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;

namespace TestApp.Samples.Adorners;

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