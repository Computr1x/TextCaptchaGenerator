using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Transform
{
    public class Scale : IEffect
    {
        private float sx, sy;
        public Scale(float s)
        {
            sx = sy = s;
        }

        public Scale(float sx, float sy)
        {
            this.sx = sx;
            this.sy = sy;
        }

        public void Draw(SKBitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();
                float x = 0, y = 0;

                int newWidth = (int)MathF.Round(width * sx), newHeight = (int)MathF.Round(height * sy);
                // Console.WriteLine($"{width};{height};{newWidth};{newHeight}");
                for (int j = 0; j < newHeight; j++)
                {
                    y = (int)(j / sy);
                    for (int i = 0; i < newWidth; i++)
                    {
                        x = (int)(i / sx);

                        // Console.WriteLine($"{x},{y},{i},{j}");

                        if (i >= 0 && i < width && j >= 0 && j < height)
                            Utils.SetColorCheckSrc(pSrc, ref width, ref height, ref x, ref y,
                                ref buffer, ref i, ref j);
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
