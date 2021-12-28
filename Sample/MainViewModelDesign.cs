using System.Collections.Generic;
using AvaloniaApplication1.ViewModels;
using CSharpFunctionalExtensions;

namespace AvaloniaApplication1
{
    public class MainViewModelDesign :IMainWindowViewModel
    {
        public IList<VideoViewModel> Videos { get; }
        public VideoViewModel SelectedVideo { get; set; }
    }
}