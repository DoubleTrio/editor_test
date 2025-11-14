using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Styling;
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


public class ToLocaleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Models.Locale.Supported.Find(x => x.Key == value as string);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (value as Models.Locale)?.Key;
    }
}

public static class StringConverters
{
    public static readonly ToLocaleConverter ToLocale = new ToLocaleConverter();

    public class ToThemeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var theme = (string)value;
            if (string.IsNullOrEmpty(theme))
                return ThemeVariant.Default;

            if (theme.Equals("Light", StringComparison.OrdinalIgnoreCase))
                return ThemeVariant.Light;

            if (theme.Equals("Dark", StringComparison.OrdinalIgnoreCase))
                return ThemeVariant.Dark;

            return ThemeVariant.Default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value as ThemeVariant)?.Key;
        }
    }
    public static readonly ToThemeConverter ToTheme = new ToThemeConverter();
}


public class WindowStateToColumnConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is WindowState ws)
        {
            if (ws == WindowState.FullScreen || !OperatingSystem.IsMacOS())
                return 0;
            
        }
        return 1;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}


// Note: This is a workaround for not being able to set the selected page to null for Tabalonia
// and so we make the selected tab seems unselected... 
public class ObjectToColorKeyConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values.Count < 2)
            throw new Exception("Expected at least 2 values: object, mandatoryKey, [optionalKey]");

        var value = values[0];
        var mandatoryKey = values[1] as string;
        var optionalKey = values.Count > 2 ? values[2] as string : null;
        
        if (mandatoryKey == null)
            throw new Exception("Mandatory key must be a string");
        
        if (value == null)
        {
            if (Application.Current?.TryGetResource(mandatoryKey, 
                    Application.Current.ActualThemeVariant, out var brush) == true)
            {
                return brush;
            }
            
        }
        
        if (optionalKey == null) return AvaloniaProperty.UnsetValue;
        
        
        if (Application.Current?.TryGetResource(optionalKey, 
                Application.Current.ActualThemeVariant, out var brushWhenNotNull) == true)
        {
            return brushWhenNotNull;
        }
      
        
        return Brushes.Transparent;
        throw new Exception($"No brush found!");
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

public class IsTypeConverter : IValueConverter
{

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        if (parameter is Type typeParam)
            return typeParam.IsInstanceOfType(value);
        
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
