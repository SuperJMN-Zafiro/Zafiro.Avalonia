using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace Zafiro.Avalonia.Misc;

public static class BitmapFactory
{
    public static Bitmap LoadFromResource(Uri resourceUri) => new(AssetLoader.Open(resourceUri));

    public static async Task<Bitmap?> LoadFromWeb(Uri url)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(url).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            return new Bitmap(new MemoryStream(data));
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"An error occurred while downloading image '{url}' : {ex.Message}");
            return null;
        }
    }
}