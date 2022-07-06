using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Color
{
    public class GrayScale : IEffect
    {
        public void Draw(SKBitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                uint curColor = 0;
                byte resColor = 0, curAlpha = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        curColor = *(pSrc + y * width + x);
                        curAlpha = (byte)(curColor >> 24);

                        if (curAlpha <= 0) continue;
                        resColor = (byte)(0.299f * (curColor >> 16 & 0xff) +
                                          0.587f * (curColor >> 8 & 0xff) +
                                          0.114f * (curColor & 0xff));

                        buffer[y, x] = Utils.MakePixel(curAlpha, resColor, resColor, resColor);
                    }
                }

                fixed (uint* newPtr = buffer)
                {
                    bitmap.SetPixels((IntPtr)newPtr);
                }
            }
        }
    }
}
