using System.Runtime.InteropServices;
using Avalonia.Platform;
using LibVLCSharp.Shared;

namespace Zafiro.Avalonia.LibVLCSharp
{
    public static class MediaPlayerExtensions
    {
        public static void DisposeHandle(this MediaPlayer player)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                player.Hwnd = IntPtr.Zero;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                player.XWindow = 0;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) player.NsObject = IntPtr.Zero;
        }

        public static void SetHandle(this MediaPlayer player, IPlatformHandle handle)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                player.Hwnd = handle.Handle;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                player.XWindow = (uint) handle.Handle;
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) player.NsObject = handle.Handle;
        }
    }
}