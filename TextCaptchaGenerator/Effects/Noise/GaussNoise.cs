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
        private Random _rand;
        // public range 0 - 255
        public float Amount
        {
            get => _amount;
            set
            {
                _amount = 1-(value % 256 / 255f);
            }
        }

        public bool Monochrome { get; set; } = false;


        // public range 0 - 255
        public GaussNoise(int seed = 0, byte amount = 255)
        {
            _rand = new Random(seed);
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

                uint colorSrc;
                float u1 = 0, u2 = 0, randStdNormal = 0;
                byte r = 0, g = 0, b = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        colorSrc = *(pSrc + (y * width + x));
                        if (colorSrc >> 24 == 0)
                            continue;

                        if (Monochrome)
                        {
                            //color = (byte)(GetNextGaussian(rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal) * 255);
                            buffer[y, x] = Utils.MultiplyFloatToPixelWithoutAlpha(
                                GetNextGaussian(_rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal), 
                                colorSrc);
                        }
                        else
                        {
                            r = (byte)(GetNextGaussian(_rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal) * 255);
                            g = (byte)(GetNextGaussian(_rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal) * 255);
                            b = (byte)(GetNextGaussian(_rand, ref mean, ref stdDev, ref u1, ref u2, ref randStdNormal) * 255);

                            buffer[y, x] = Utils.MultiplyPixelToPixel(Utils.MakePixel(255, r, g, b), colorSrc);
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

