using System.Collections.Generic;
using Avalonia.Controls.Selection;
using Zafiro.Avalonia;

namespace TestApp.Samples.Controls;

public class ControlsSampleViewModel
{
    public ControlsSampleViewModel()
    {
        SelectionModel = new SelectionModel<Item> { SingleSelect = false };
        Items = new List<Item> { new Item(1), new Item(2), new Item(3) };
        SelectionHandler = new SelectionHandler<Item, int>(SelectionModel, arg => arg.Id);
    }

    public List<Item> Items { get; }

    public SelectionHandler<Item, int> SelectionHandler { get; }

    public SelectionModel<Item> SelectionModel { get; }
}