using Avalonia.Data.Converters;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace Zafiro.Avalonia.Converters;

public class ImageConverters
{
    public static FuncValueConverter<byte[], IImage> ByteArrayToBitmapImage = new FuncValueConverter<byte[], IImage>(bytes => LoadBitmapFromBytes(bytes));
    
    public static Bitmap LoadBitmapFromBytes(byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
            throw new ArgumentException("El array de bytes no puede estar vacío.", nameof(imageData));

        using var stream = new MemoryStream(imageData); // El Stream solo vive durante este bloque
        return new Bitmap(stream); // Bitmap copia los datos y ya no necesita el Stream
    }
    
    public static readonly FuncValueConverter<Uri, Bitmap?> UriToBitmap = new(uri =>
    {
        if (uri == null)
        {
            return null;
        }

        return ImageExtensions.ImageHelper.LoadFromResource(uri);
    });
}