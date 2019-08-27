using System;
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace WpfApp4
{
    //setting file/folder icon from windows
    #region HeaderToImageConverter

    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();
        

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try { 
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
            catch (NullReferenceException e1)
            {
                Console.WriteLine(e1.InnerException);
                return new object();
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }

    #endregion // DoubleToIntegerConverter
}