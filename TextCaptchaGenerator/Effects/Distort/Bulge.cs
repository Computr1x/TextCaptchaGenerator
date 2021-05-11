using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Distort
{
    public class Bulge : IEffect
    {
        public int X { get; }
        public int Y { get; }
        public float Radius { get; }
        public float Strenght { get; }
        public bool Antialiasing { get; set; } = true;

        public Bulge(int x, int y, float radius, float strenght)
        {
            X = x;
            Y = y;
            Radius = radius;
            Strenght = strenght;
        }

        public void Draw(SKBitmap bitmap)
        {
            void CalculateBulge(ref int x, ref int y, ref float interpolationFactor,
                ref float pixelDistance, ref float pixelAngle,
                ref float pixelX, ref float pixelY,
                ref float xOffset, ref float yOffset)
            {
                pixelX = x - X;
                pixelY = y - Y;

                pixelDistance = MathF.Sqrt(pixelX * pixelX + pixelY * pixelY);
                pixelAngle = MathF.Atan2(pixelY, pixelX);

                interpolationFactor = pixelDistance / Radius;
                pixelDistance = interpolationFactor * pixelDistance + (1.0f - interpolationFactor) * Strenght * MathF.Sqrt(pixelDistance);

                xOffset = MathF.Cos(pixelAngle) * pixelDistance + X;
                yOffset = MathF.Sin(pixelAngle) * pixelDistance + Y;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();
                float pixelX = 0, pixelY = 0, offsetX = 0, offsetY = 0, pixelDistance = 0, pixelAngle = 0;
                int r = (int) Radius;

                float d = 0, interpolationFactor = 0;
                // wave with antialiasing
                if (Antialiasing)
                {
                    float fractionX = 0, fractionY = 0, oneMinusX = 0, oneMinusY = 0;
                    int ceilX = 0, ceilY = 0, floorX = 0, floorY = 0;

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            d = MathF.Sqrt(MathF.Pow(x - X, 2f) + MathF.Pow(y - Y, 2f));
                            if (d <= r)
                            {
                                CalculateBulge(ref x, ref y, ref interpolationFactor, ref pixelDistance, ref pixelAngle,
                                        ref pixelX, ref pixelY, ref offsetX, ref offsetY);
                                Utils.SetAntialisedColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                    ref buffer, ref x, ref y,
                                    ref floorX, ref floorY, ref ceilX, ref ceilY, ref fractionX,
                                    ref fractionY, ref oneMinusX, ref oneMinusY);
                            }
                            else
                            {
                                Utils.SetColor(pSrc, ref width, ref height, ref buffer, ref x, ref y);
                            }
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
                            d = MathF.Sqrt(MathF.Pow(x - X, 2f) + MathF.Pow(y - Y, 2f));
                            if (d <= r)
                            {
                                CalculateBulge(ref x, ref y, ref interpolationFactor, ref pixelDistance, ref pixelAngle,
                                        ref pixelX, ref pixelY, ref offsetX, ref offsetY);
                                Utils.SetColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                    ref buffer, ref x, ref y);
                            }
                            else
                            {
                                Utils.SetColor(pSrc, ref width, ref height, ref buffer, ref x, ref y);
                            }                          
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
