using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace AvaloniaTest.Converters;

public class SvgKeyToPathConverter : IValueConverter
 {
     public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
     {
         if (value is string key && !string.IsNullOrEmpty(key))
         {
             string res = Application.Current?.Resources[key] as string;
             return res;
         }
         return null;
     }
 
     public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
         throw new NotImplementedException();
 }