using System;
using System.Collections.ObjectModel;
using System.Linq;
using DynamicData;
using Zafiro.UI.Shell.Utils;

namespace TestApp.Samples.Misc;

[Section("Visibility Aware Items", icon: "mdi-eye-outline", sortIndex: 7)]
public class VisibilityAwareItemsControlViewModel
{
    public VisibilityAwareItemsControlViewModel()
    {
        var list = new SourceList<DemoItem>();

        // Create a bunch of items with varying heights to better visualize partial visibility
        var items = Enumerable.Range(1, 60)
            .Select(i => new DemoItem($"Item {i}", Height: 30 + (i % 6) * 15))
            .ToList();

        list.AddRange(items);
        list.Connect()
            .Bind(out var ro)
            .Subscribe();
        Items = ro;
    }

    public ReadOnlyObservableCollection<DemoItem> Items { get; }
}

public record DemoItem(string Title, double Height);