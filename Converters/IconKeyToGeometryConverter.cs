using Avalonia;
using Avalonia.Data.Converters;

namespace AvaloniaTest.Converters;

using Avalonia.Media;
using System;
using System.Globalization;

public class IconKeyToGeometryConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string key && Application.Current?.Resources.TryGetResource(key, null, out var geo) == true)
        {
            return geo as StreamGeometry;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}