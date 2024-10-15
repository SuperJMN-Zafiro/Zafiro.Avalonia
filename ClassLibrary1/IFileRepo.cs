using CSharpFunctionalExtensions;
using Zafiro.FileSystem.Core;

namespace ClassLibrary1;

public interface IFileRepo 
{
    public Task<Result<INode>> Get(FileLocator item);
}