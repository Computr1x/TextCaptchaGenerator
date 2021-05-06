using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Distorsions
{
    public class Wave : IEffect
    {
        public float WaveLength { get; }
        public float Amplitude { get; }
        public bool Antialiasing { get; set; } = true;

        public Wave(float waveLength, float amplitude)
        {
            WaveLength = waveLength;
            Amplitude = amplitude;
        }

        public void Draw(SKBitmap bitmap)
        {
            void CalculateWave(ref int x, ref int y,
                ref float pixelX, ref float pixelY, 
                ref float xOffset, ref float yOffset)
            {
                pixelX = (Amplitude * MathF.Sin(2.0f * MathF.PI * y / WaveLength));
                pixelY = (Amplitude * MathF.Cos(2.0f * MathF.PI * x / WaveLength));

                xOffset = x + pixelX;
                yOffset = y + pixelY;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[width, height];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();
                float pixelX = 0, pixelY = 0, offsetX = 0, offsetY = 0;

                // wave with antialiasing
                if (Antialiasing)
                {
                    float fractionX = 0, fractionY = 0, oneMinusX = 0, oneMinusY = 0;
                    int ceilX = 0, ceilY = 0, floorX = 0, floorY = 0;

                    for (int x = 0; x < height; x++)
                    {
                        for (int y = 0; y < width; y++)
                        {
                            CalculateWave(ref x, ref y, ref pixelX, ref pixelY, ref offsetX, ref offsetY);
                            Utils.SetAntialisedColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                ref buffer, ref x, ref y,
                                ref floorX, ref floorY, ref ceilX, ref ceilY, ref fractionX,
                                ref fractionY, ref oneMinusX, ref oneMinusY);
                        }
                    }
                }
                // wave without antialiasing
                else
                {
                    for (int x = 0; x < height; x++)
                    {
                        for (int y = 0; y < width; y++)
                        {
                            CalculateWave(ref x, ref y, ref pixelX, ref pixelY, ref offsetX, ref offsetY);
                            Utils.SetColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                ref buffer, ref x, ref y);
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
