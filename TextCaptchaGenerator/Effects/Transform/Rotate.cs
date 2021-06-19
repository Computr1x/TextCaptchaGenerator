using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Transform
{
    public class Rotate : IEffect
    {
        private float degrees;

        public Rotate(float degrees)
        {
            this.degrees = degrees * MathF.PI / 180f;
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
                float sinA = MathF.Sin(degrees), cosA = MathF.Cos(degrees), centerX = width / 2f, centerY = height / 2f, offsetX = 0, offsetY = 0;
                float fromCenterX = 0, fromCenterY = 0;

                for (int y = 0; y < height; y++)
                {
                    fromCenterY = y - centerY;
                    for (int x = 0; x < width; x++)
                    {
                        fromCenterX = x - centerX;

                        offsetX = cosA * fromCenterX + sinA * fromCenterY + centerX;
                        offsetY = -sinA * fromCenterX + cosA * fromCenterY + centerY;

                        Utils.SetColorCheckSrc(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                ref buffer, ref x, ref y);
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