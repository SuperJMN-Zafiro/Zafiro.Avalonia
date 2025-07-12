using System.Net.Http;
using Serilog;

namespace SampleFileExplorer;

public static class LoggerExtensions
{
    public static HttpMessageHandler GetHandler(ILogger logger)
    {
#if DEBUG
        if (OperatingSystem.IsWindows() || OperatingSystem.IsLinux())
        {
            LoggingHttpMessageHandler handler = new LoggingHttpMessageHandler(new SerilogToMicrosoftLoggerAdapter(logger));   
            handler.InnerHandler = new HttpClientHandler();
            return handler;
        }
        else
        {
            return new HttpClientHandler();
        }
#else
        return new HttpClientHandler();
#endif
    }
}