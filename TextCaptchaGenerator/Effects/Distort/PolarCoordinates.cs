using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Distort
{
    public class PolarCoordinates : IEffect
    {
        public enum eConversionType { RectangularToPolar, PolarToRectangular };

        public eConversionType PolarType { get; } = eConversionType.RectangularToPolar;
        public bool Antialiasing { get; set; } = true;

        public PolarCoordinates() { }

        public PolarCoordinates(eConversionType polarType)
        {
            PolarType = polarType;
        }

        delegate void CalcPolarDelegate(ref int x, ref int y, ref float scaleX, ref float scaleY, ref float centerX, ref float centerY,
                ref float pixelX, ref float pixelY, ref float offsetX, ref float offsetY);

        public void Draw(SKBitmap bitmap)
        {
            void RectangularToPolar(ref int x, ref int y, ref float scaleX, ref float scaleY, ref float centerX, ref float centerY,
                ref float pixelX, ref float pixelY, ref float offsetX, ref float offsetY)
            {
                pixelX = x - centerX;
                pixelY = y - centerY;

                // pixelDistance = MathF.Sqrt(pixelX * pixelX + pixelY * pixelY)
                // pixelAngle = MathF.Atan2(pixelY, pixelX)

                offsetX = MathF.Sqrt(pixelX * pixelX + pixelY * pixelY) * scaleX;
                offsetY = (MathF.Atan2(pixelY, pixelX) + MathF.PI) /*% (2f * MathF.PI)*/ * scaleY;
            }

            void PolarToRectangular(ref int x, ref int y, ref float scaleX, ref float scaleY, ref float centerX, ref float centerY,
                ref float pixelX, ref float pixelY, ref float offsetX, ref float offsetY)
            {
                pixelX = x / scaleX;
                pixelY = y / scaleY - MathF.PI;

                offsetX = pixelX * MathF.Cos(pixelY) + centerX;
                offsetY = pixelX * MathF.Sin(pixelY) + centerY;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                // maxRadius = MathF.Min(width, height) / 2f, MathF.Sqrt(width * width + height * height) / 4f
                uint* pSrc = (uint*)pixelsAddr.ToPointer();
                float centerX = width / 2f, centerY = height / 2f,
                    pixelX = 0, pixelY = 0, offsetX = 0, offsetY = 0,
                    maxRadius = MathF.Sqrt(width * width + height * height) / 4f,
                    scaleX = width / maxRadius, scaleY = height / (2f * MathF.PI);

                CalcPolarDelegate calcPolar = PolarType switch
                {
                    eConversionType.RectangularToPolar => RectangularToPolar,
                    _ => PolarToRectangular,
                };

                float fractionX = 0, fractionY = 0, oneMinusX = 0, oneMinusY = 0;
                int ceilX = 0, ceilY = 0, floorX = 0, floorY = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        calcPolar(ref x, ref y, ref scaleX, ref scaleY, ref centerX, ref centerY,
                            ref pixelX, ref pixelY, ref offsetX, ref offsetY);

                        if (Antialiasing)
                            Utils.SetAntialisedColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                ref buffer, ref x, ref y,
                                ref floorX, ref floorY, ref ceilX, ref ceilY, ref fractionX,
                                ref fractionY, ref oneMinusX, ref oneMinusY);
                        else
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
