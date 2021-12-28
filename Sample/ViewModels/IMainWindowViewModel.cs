using System.Collections.Generic;

namespace AvaloniaApplication1.ViewModels
{
    public interface IMainWindowViewModel
    {
        IList<VideoViewModel> Videos { get; }
        VideoViewModel SelectedVideo { get; set; }
    }
}