using Avalonia.Media;

namespace Zafiro.Avalonia.Controls.Panels;

public partial class AdaptivePanelExample : UserControl
{
    public AdaptivePanelExample()
    {
        InitializeComponent();
    }

    private void OnOverflowStateChanged(object? sender, OverflowStateChangedEventArgs e)
    {
        if (StatusText != null)
        {
            StatusText.Text = e.IsOverflow ? "Overflow" : "Normal";
            StatusText.Foreground = e.IsOverflow ? Brushes.Red : Brushes.Green;
        }
    }
}