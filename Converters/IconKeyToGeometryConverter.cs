using Avalonia;
using Avalonia.Data.Converters;
using AvaloniaTest.ViewModels;

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

public class IsTypeConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        Console.WriteLine("AAA");
        if (value == null || parameter == null)
            return false;

        if (parameter is Type typeParam)
            return typeParam.IsInstanceOfType(value);

        // Optionally, support string type name
        if (parameter is string typeName)
            return value.GetType().Name == typeName;

        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

public class NodeTitleConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is NodeBase node)
            return node.Title;

        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
