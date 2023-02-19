using Avalonia.Controls;
using Zafiro.UI;

namespace Zafiro.Avalonia;

public static class FilePicker
{
    public static List<FileDialogFilter> Map(params FileTypeFilter[] filters)
    {
        return filters.Select(tuple => new FileDialogFilter
        {
            Name = tuple.Description,
            Extensions = tuple.Extensions.ToList()
        }).ToList();
    }
}