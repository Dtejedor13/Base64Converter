using Base64ConverterCore.Interfaces;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Base64ConverterCore.Core
{
    public class Base64ToImageConverter : IMediaConverter
    {
        public object ConvertToObject(string value)
        {
            byte[] bytes = Convert.FromBase64String(value);
            Image image;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                image = Image.FromStream(ms);
            }

            return image;
        }

        public string ConvertToString(object value)
        {
            Image img = (Image)value;
            var imageStream = new MemoryStream();
            try
            {
                img.Save(imageStream, ImageFormat.Bmp);
                imageStream.Position = 0;
                var imageBytes = imageStream.ToArray();
                var ImageBase64 = Convert.ToBase64String(imageBytes);
                return ImageBase64;
            }
            catch (Exception ex)
            {
                return "Error converting image to base64!";
            }
        }
    }
}
