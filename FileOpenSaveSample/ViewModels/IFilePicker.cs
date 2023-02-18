using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Zafiro.FileSystem;

namespace FileOpenSaveSample.ViewModels;

public interface IFilePicker
{
    IObservable<IEnumerable<Result<IZafiroFile>>> PickMultiple(params (string, string[])[] filters);
    IObservable<Result<IZafiroFile>> PickSingle(params (string, string[])[] filters);
}