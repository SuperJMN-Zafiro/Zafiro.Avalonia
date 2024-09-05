using Zafiro.Avalonia.FileExplorer.Core.Clipboard;

namespace Zafiro.Avalonia.FileExplorer.Core;

public record FileSystemConnection(string Identifier, string Name, IMutableFileSystem FileSystem) : IConnection;