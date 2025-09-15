using Avalonia.Controls;
using Avalonia.Interactivity;

namespace TestApp.Samples.Behaviors;

public partial class ChildrenVisibilityTrackerView : UserControl
{
    public ChildrenVisibilityTrackerView()
    {
        InitializeComponent();
    }

    private void HideItem3_Click(object? sender, RoutedEventArgs e)
    {
        var item3 = TestPanel.Children[2]; // Third item (Item 3)
        if (item3 is Control control)
        {
            control.IsVisible = false;
        }
    }

    private void ShowItem3_Click(object? sender, RoutedEventArgs e)
    {
        var item3 = TestPanel.Children[2]; // Third item (Item 3)
        if (item3 is Control control)
        {
            control.IsVisible = true;
        }
    }
}
