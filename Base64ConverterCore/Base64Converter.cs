using Base64ConverterCore.Core;
using System.Drawing;

namespace Base64ConverterCore
{
    public class Base64Converter : GifConverterBase
    {
        #region converter modules
        private Base64ToBitmapConverter bitmapConverter;
        private Base64ToImageConverter imageConverter;
        #endregion

        public Base64Converter()
        {
            // build modules
            bitmapConverter = new Base64ToBitmapConverter();
            imageConverter = new Base64ToImageConverter();
        }

        #region convert to object
        public Bitmap ConvertBase64ToBitmap(string value)
        {
            return (Bitmap)bitmapConverter.ConvertToObject(value);
        }

        public Image ConvertBase64ToImage(string value)
        {
            return (Image)imageConverter.ConvertToObject(value);
        }
        #endregion

        #region convert to base64
        public string ConvertToBase64(Bitmap value)
        {
            return bitmapConverter.ConvertToString(value);
        }

        public string ConvertToBase64(Image value)
        {
            return imageConverter.ConvertToString(value);
        }
        #endregion

        public Bitmap[] ConvertGifToBitmaps(Image value)
        {
            Bitmap targetType = null;
            return (Bitmap[])extractFramesFromGif(targetType, value);
        }
        public Image[] ConvertGifToImages(Image value)
        {
            Image targetType = null;
            return (Image[])extractFramesFromGif(targetType, value);
        }
    }
}
