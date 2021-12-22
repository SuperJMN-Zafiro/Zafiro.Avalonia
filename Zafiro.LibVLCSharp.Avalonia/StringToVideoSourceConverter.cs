using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using CSharpFunctionalExtensions;

namespace Zafiro.Avalonia.LibVLCSharp;

public class StringToVideoSourceConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is null)
        {
            return Maybe<VideoSource>.None;
        }

        if (value is string str)
        {
            return Maybe.From(new VideoSource(str));
        }

        return BindingValue<VideoSource>.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

