using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Noise
{
    public class GaussNoise : IEffect
    {
        // internal range 0 - 1
        private float _amount = 100f / 400f;
        // public range 0 - 255
        public float Amount
        {
            get => _amount;
            set
            {
                _amount = 1-(value % 256 / 256f);
            }
        }

        public bool Monochrome { get; set; } = false;

        public GaussNoise()
        {
        }

        // public range 0 - 256
        public GaussNoise(byte amount)
        {
            Amount = amount;
        }

        public void Draw(SKBitmap bitmap)
        {
            float GetNextGaussian(Random rand, ref float mean, ref float stdDev, ref float u1, ref float u2, ref float randStdNormal)
            {
                //uniform(0,1] random doubles
                u1 = 1.0f - (float)rand.NextDouble();
                u2 = 1.0f - (float)rand.NextDouble();
                randStdNormal = MathF.Sqrt(-2.0f * MathF.Log(u1)) *
                             MathF.Sin(2.0f * MathF.PI * u2);
                //random normal (0,1)
                return (mean + stdDev * randStdNormal);
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            //float mean = Amount, stdDev = (1f - mean) / 6f;
            float mean = Amount, stdDev = (1f - mean) / 6f;

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                Random rand = new Random();
                float u1 = 0, u2 = 0, randStdNormal = 0;


                byte color = 0, r = 0, g = 0, b = 0;
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        uint colorSrc = *(pSrc + (y * width + x));
                        if (colorSrc >> 24 == 0)
                            continue;

                        if (Monochrome)
                        {
                            color = (byte)(GetNextGaussian(rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal) * 255);
                            buffer[y, x] = Utils.MultiplyFloatToPixel(Utils.MakePixel(color, color, color, 255), colorSrc);
                        }
                        else
                        {
                            r = (byte)(GetNextGaussian(rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal) * 255);
                            g = (byte)(GetNextGaussian(rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal) * 255);
                            b = (byte)(GetNextGaussian(rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal) * 255);

                            buffer[y, x] = Utils.MultiplyFloatToPixel(Utils.MakePixel(r, g, b, 255), colorSrc);
                        }
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

