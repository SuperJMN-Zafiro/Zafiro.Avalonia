using System;
using System.Net.Http;
using HttpClient.Extensions.LoggingHttpMessageHandler;
using Serilog;
using Zafiro.Misc;

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