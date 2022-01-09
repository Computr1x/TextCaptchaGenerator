using SkiaSharp;
using System;
using System.Linq;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Glitch
{
    public class Crystallize : IEffect
    {
        private readonly Random _r;
        public int CrystalsCount { get; set; } = 64;


        public Crystallize(int seed = 0)
        {
            _r = new Random(seed);
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

                // x centers
                uint[] xn = Enumerable.Range(0, CrystalsCount).Select(x => (uint)_r.Next(0, width)).ToArray();
                // y centers
                uint[] yn = Enumerable.Range(0, CrystalsCount).Select(x => (uint)_r.Next(0, height)).ToArray();

                uint d = 0, dMin = 0, dIndex = 0; ;


                for (uint y = 0; y < height; y++)
                {
                    for (uint x = 0; x < width; x++)
                    {
                        dMin = uint.MaxValue;

                        for (uint i = 0; i < CrystalsCount; i++)
                        {
                            d = (y - yn[i]) * (y - yn[i]) + (x - xn[i]) * (x - xn[i]);

                            if (d >= dMin) continue;
                            dMin = d;
                            dIndex = i;
                        }

                        buffer[y, x] = *(pSrc + yn[dIndex] * width + xn[dIndex]);
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
