using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Transform
{
    public class Shift : IEffect
    {
        private int sx, sy;

        public bool RepeatPixels { get; set; } = true;
        public Shift(int s)
        {
            sx = sy = s;
        }

        public Shift(int sx, int sy)
        {
            this.sx = sx;
            this.sy = sy;
        }

        public void Draw(SKBitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            sx %= width;
            sy %= height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                if (RepeatPixels)
                {
                    for (int y = 0, offsetY = sy; y < height; y++, offsetY++)
                    {
                        for (int x = 0, offsetX = sx; x < width; x++, offsetX++)
                        {
                            if (offsetX >= width)
                                offsetX = 0;
                            else if (offsetX < 0)
                                offsetX = width + sx;
                            if (offsetY >= height)
                                offsetY = 0;
                            else if (offsetY < 0)
                                offsetY = height + sy;

                            Utils.SetColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                    ref buffer, ref x, ref y);
                        }
                    }
                }
                            
                else
                {
                    for (int x = 0, offsetX = -sx; offsetX < width - sx; x++, offsetX++)
                        for (int y = 0, offsetY = -sy; offsetY < height - sy; y++, offsetY++)
                            Utils.SetColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                    ref buffer, ref x, ref y);
                }

                fixed (uint* newPtr = buffer)
                {
                    bitmap.SetPixels((IntPtr)newPtr);
                }
            }
        }
    }
}
