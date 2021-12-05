using System.Collections.Generic;
using Zafiro.Avalonia.LibVLCSharp;

namespace VideoTest.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {
            var mediaPlayerFactory = new MediaPlayerFactory();

            Videos = new List<VideoViewModel>()
            {
                new("https://filesamples.com/samples/video/mp4/sample_640x360.mp4", mediaPlayerFactory),
                new("https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/360/Big_Buck_Bunny_360_10s_30MB.mp4", mediaPlayerFactory),
            };
        }

        public IEnumerable<VideoViewModel> Videos { get;  }
    }
}
