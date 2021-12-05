using LibVLCSharp.Shared;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public class Vlc
    {
        private static readonly LibVLC instance;

        static Vlc()
        {
            instance = new LibVLC();
        }

        public static LibVLC Instance => instance;
    }
}