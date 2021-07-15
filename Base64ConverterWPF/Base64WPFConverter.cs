using Base64ConverterCore;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Base64ConverterWPF
{
    public class Base64WPFConverter : Base64Converter
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public ImageSource Convert(Bitmap value)
        {
            var handle = value.GetHbitmap();
            try
            {
                return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
            }
            finally { DeleteObject(handle); }
        }

        public ImageSource Convert(Image value)
        {
            return Convert(new Bitmap(value));
        }

        public ImageSource Convert(string base64)
        {
            return Convert(new Bitmap(ConvertBase64ToBitmap(base64)));
        }
    }
}
