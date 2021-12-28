using System.Collections.Generic;
using System.Text;
using ReactiveUI;
using Zafiro.Avalonia.LibVLCSharp;

namespace AvaloniaApplication1.ViewModels
{
    public class MainWindowViewModel : ViewModelBase, IMainWindowViewModel
    {
        private VideoViewModel selectedVideo;
        public string Greeting => "Welcome to Avalonia!";

        public MainWindowViewModel()
        {
            var factory = new MediaFactory();

            Videos = new List<VideoViewModel>()
            {
                new VideoViewModel(
                    "https://test-videos.co.uk/vids/bigbuckbunny/mp4/h264/1080/Big_Buck_Bunny_1080_10s_1MB.mp4", factory),
                new VideoViewModel(
                    "https://file-examples-com.github.io/uploads/2017/04/file_example_MP4_1920_18MG.mp4", factory),
            };
        }

        public IList<VideoViewModel> Videos { get; }

        public VideoViewModel SelectedVideo
        {
            get => selectedVideo;
            set => this.RaiseAndSetIfChanged(ref selectedVideo, value);
        }
    }
}
