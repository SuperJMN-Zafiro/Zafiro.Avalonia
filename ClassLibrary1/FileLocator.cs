using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.FileSystem.Core;

namespace ClassLibrary1;

public record FileLocator(string Connection, ZafiroPath Path, ItemType ItemType);