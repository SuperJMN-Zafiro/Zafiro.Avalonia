using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Zafiro.Avalonia.Misc;

public static class BitmapFactory
{
    public static Bitmap LoadFromResource(Uri resourceUri) => new(AssetLoader.Open(resourceUri));
    
    public static Bitmap Load(byte[] bytes)
    {
        using (var memoryStream = new MemoryStream(bytes))
        {
            return new Bitmap(memoryStream);
        }
    }

    public static async Task<Bitmap?> LoadFromWeb(Uri url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            await using (var memoryStream = new MemoryStream(data))
            {
                return new Bitmap(memoryStream);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
}