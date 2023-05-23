using Avalonia.Platform.Storage;
using Zafiro.UI;

namespace Zafiro.Avalonia.Storage;

public static class FilePicker
{
    public static IReadOnlyList<FilePickerFileType> Map(params FileTypeFilter[] filters)
    {
        return filters.Select(tuple => new FilePickerFileType(tuple.Description)
        {
            Patterns = tuple.Extensions
        }).ToList();
    }
}