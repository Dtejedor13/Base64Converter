using Base64ConverterCore.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Base64ConverterCore.Core
{
    public class Base64ToBitmapConverter : IMediaConverter
    {
        public object ConvertToObject(string value)
        {
            Bitmap bmpReturn = null;
            byte[] byteBuffer = Convert.FromBase64String(value);
            MemoryStream memoryStream = new MemoryStream(byteBuffer);

            memoryStream.Position = 0;

            bmpReturn = (Bitmap)Bitmap.FromStream(memoryStream);

            memoryStream.Close();

            return bmpReturn;
        }

        public string ConvertToString(object value)
        {
            Bitmap bmp = (Bitmap)value;
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            byte[] byteImage = ms.ToArray();
            return Convert.ToBase64String(byteImage);
        }
    }
}
