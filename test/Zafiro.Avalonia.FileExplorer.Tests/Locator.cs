using Zafiro.Avalonia.FileExplorer.Core.Transfers;
using Zafiro.FileSystem.Core;

namespace Zafiro.Avalonia.FileExplorer.Tests;

public record Locator(string Connection, ZafiroPath Path, ItemType ItemType);