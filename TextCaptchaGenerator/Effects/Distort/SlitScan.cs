using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Distort
{
    public class SlitScan : IEffect
    {
        public bool Antialiasing { get; set; } = true;


        float Mix(float a, float b, float l)
        {
            return a + (b - a) * l;
        }

        float UpDown(float v)
        {
            return MathF.Sin(v) * 0.5f + 0.5f;
        }

        public void Draw(SKBitmap bitmap)
        {
            float t1 = 1.5f, t2 = t1 * 0.37f;


            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();
                float pixelX = 0, pixelY = 0, offsetX = 0, offsetY = 0, pixelDistance = 0, pixelAngle = 0;

                float v = 0, offset = 0, offset1 = 0, offset2 = 0;

                // meow
                for (int y = 0; y < width; y++)
                {
                    v = y / (float) height;

                    offset1 = MathF.Sin((v + 0.5f) * Mix(3f, 12f, UpDown(t1))) * 15;
                    offset2 = MathF.Sin((v + 0.5f) * Mix(3f, 12f, UpDown(t2))) * 15;
                    offset = offset1 + offset2;

                    offsetY = y * height / (float) height + offset;
                    offsetY = Math.Max(0, Math.Min(height - 1, offsetY));

                    for (int x = 0; x < height; x++)
                    {
                        offsetX = x;
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
