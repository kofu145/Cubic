using System;

namespace Cubic.Utilities;

public partial class Bitmap
{
    public static Bitmap ConvertToColorSpace(Bitmap bitmap, ColorSpace colorSpace)
    {
        if (colorSpace == bitmap.ColorSpace)
            return bitmap;

        switch (colorSpace)
        {
            case ColorSpace.Unsupported:
                throw new CubicException("Not supported.");
                break;
            case ColorSpace.RGB:
                byte[] dataRGB = new byte[bitmap.Size.Width * bitmap.Size.Height * 3];
                for (int x = 0; x < bitmap.Size.Width; x++)
                {
                    for (int y = 0; y < bitmap.Size.Height; y++)
                    {
                        dataRGB[(y * bitmap.Size.Width + x) * 3] = bitmap.Data[(y * bitmap.Size.Width + x) * 4];
                        dataRGB[(y * bitmap.Size.Width + x) * 3 + 1] = bitmap.Data[(y * bitmap.Size.Width + x) * 4 + 1];
                        dataRGB[(y * bitmap.Size.Width + x) * 3 + 2] = bitmap.Data[(y * bitmap.Size.Width + x) * 4 + 2];
                    }
                }

                return new Bitmap(bitmap.Size.Width, bitmap.Size.Height, dataRGB, ColorSpace.RGB);
            case ColorSpace.RGBA:
                byte[] dataRGBA = new byte[bitmap.Size.Width * bitmap.Size.Height * 4];
                for (int x = 0; x < bitmap.Size.Width; x++)
                {
                    for (int y = 0; y < bitmap.Size.Height; y++)
                    {
                        dataRGBA[(y * bitmap.Size.Width + x) * 4] = bitmap.Data[(y * bitmap.Size.Width + x) * 3];
                        dataRGBA[(y * bitmap.Size.Width + x) * 4 + 1] = bitmap.Data[(y * bitmap.Size.Width + x) * 3 + 1];
                        dataRGBA[(y * bitmap.Size.Width + x) * 4 + 2] = bitmap.Data[(y * bitmap.Size.Width + x) * 3 + 2];
                        dataRGBA[(y * bitmap.Size.Width + x) * 4 + 3] = 255;
                    }
                }

                return new Bitmap(bitmap.Size.Width, bitmap.Size.Height, dataRGBA, ColorSpace.RGBA);
            default:
                throw new ArgumentOutOfRangeException(nameof(colorSpace), colorSpace, null);
        }
    }
    
    //public void DrawImage(Bitmap image, )
}