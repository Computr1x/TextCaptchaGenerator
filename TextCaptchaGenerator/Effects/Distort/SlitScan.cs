using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Distort
{
    public class SlitScan : IEffect
    {
        public float Time { get; }

        public SlitScan()
        {
            Time = 3f;
        }

        public SlitScan(float time)
        {
            Time = time;
        }

        public void Draw(SKBitmap bitmap)
        {
            float Mix(float a, float b, float l)
            {
                return a + (b - a) * l;
            }

            float UpDown(float v)
            {
                return MathF.Sin(v) * 0.5f + 0.5f;
            }

            float t1 = Time, t2 = t1 * 0.37f;


            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();
                float offsetX = 0, offsetY = 0, v = 0, offset = 0, offset1 = 0, offset2 = 0;


                for (int y = 0; y < height; y++)
                {
                    v = y / (float)height;

                    offset1 = MathF.Sin((v + 0.5f) * Mix(3f, 12f, UpDown(t1))) * 15;
                    offset2 = MathF.Sin((v + 0.5f) * Mix(3f, 12f, UpDown(t2))) * 15;
                    offset = offset1 + offset2;

                    offsetY = y * height / (float)height + offset;
                    offsetY = Math.Max(0, Math.Min(height - 1, offsetY));

                    for (int x = 0; x < width; x++)
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
