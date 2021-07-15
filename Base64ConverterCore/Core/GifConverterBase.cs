using System.Drawing;
using System.Drawing.Imaging;

namespace Base64ConverterCore.Core
{
    public abstract class GifConverterBase
    {
        public object[] extractFramesFromGif(object targetType, Image gif)
        {
            int numberOfFrames = gif.GetFrameCount(FrameDimension.Time);
            object[] frames;

            if (targetType == typeof(Bitmap))
                frames = new Bitmap[numberOfFrames];
            else if (targetType == typeof(Image))
                frames = new Image[numberOfFrames];
            else
                return null;

            for (int i = 0; i < numberOfFrames; i++)
            {
                gif.SelectActiveFrame(FrameDimension.Time, i);

                if (targetType == typeof(Bitmap))
                    frames[i] = ((Bitmap)gif.Clone());
                else if (targetType == typeof(Image))
                    frames[i] = ((Bitmap)gif.Clone());
            }

            return frames;
        }
    }
}
