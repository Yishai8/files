﻿using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp4
{
    #region HeaderToImageConverter

    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string tag = value.ToString();
            
                Icon ic = SysIcon.OfPath(tag);

            ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
       ic.Handle,
       Int32Rect.Empty,
       BitmapSizeOptions.FromEmptyOptions());
            //var bitmap = ic.ToBitmap();
            //BitmapImage source = bitmap;
            return imageSource;
          
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }

    #endregion // DoubleToIntegerConverter
}