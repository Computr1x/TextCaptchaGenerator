using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Distort
{
    public class Ripple : IEffect
    {
        private bool customCoords = false;

        // center of Ripple, by default center of image
        public int X { get; set; }
        public int Y { get; set; }
        // radius of effect in pixels
        public float Radius { get; } =  100f;
        //  wavelength of ripples, in pixels
        public float WaveLength { get; } = 10f;
        // approximate width of wave train, in wavelengths
        public float TraintWidth { get; set; } = 2f;
        // phase vel. / group vel. (irrelevant for stills)
        //public float SuperPhase { get; set; } = 5f;
        public bool Antialiasing { get; set; } = true;

        public Ripple(int x, int y)
        {
            X = x;
            Y = y;
            customCoords = true;
        }

        public Ripple(float radius, float waveLength)
        {
            Radius = radius;
            WaveLength = waveLength;
        }

        public Ripple(int x, int y, float radius, float waveLength, float traintWidth) : this(x, y)
        {
            Radius = radius;
            WaveLength = waveLength;
            TraintWidth = traintWidth;
        }

        public void Draw(SKBitmap bitmap)
        {
            void CalculateRipple(ref int x, ref int y,
                ref float centerX, ref float centerY,
                ref float pixelX, ref float pixelY,
                ref float r, ref float k, ref float z,
                ref float offsetX, ref float offsetY)
            {
                pixelX = x - centerX;
                pixelY = y - centerY;

                // pixelDistance = MathF.Sqrt(pixelX * pixelX + pixelY * pixelY)
                // pixelAngle = MathF.Atan2(pixelY, pixelX)

                r = (MathF.Sqrt(pixelX * pixelX + pixelY * pixelY) - Radius) / WaveLength;
                z = 1f / (1f + MathF.Pow(r / TraintWidth, 2f)) * MathF.Sin(r * 2f * MathF.PI);

                offsetX = x + x * z;
                offsetY = y + y * z;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                float centerX = width / 2.0f, centerY = height / 2.0f,
                pixelX = 0, pixelY = 0, r = 0, k = 0, z = 0,
                offsetX = 0, offsetY = 0;

                if (customCoords)
                {
                    centerX = X;
                    centerY = Y;
                }

                // swirl with antialiasing
                if (Antialiasing)
                {
                    float fractionX = 0, fractionY = 0, oneMinusX = 0, oneMinusY = 0;
                    int ceilX = 0, ceilY = 0, floorX = 0, floorY = 0;

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            CalculateRipple(ref x, ref y,
                                    ref centerX, ref centerY,
                                    ref pixelX, ref pixelY,
                                    ref r, ref k, ref z,
                                    ref offsetX, ref offsetY);
                            Utils.SetAntialisedColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                    ref buffer, ref x, ref y,
                                    ref floorX, ref floorY, ref ceilX, ref ceilY, ref fractionX,
                                    ref fractionY, ref oneMinusX, ref oneMinusY);
                        }
                    }
                }
                // swirl without antialiasing
                else
                {
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            CalculateRipple(ref x, ref y,
                                   ref centerX, ref centerY,
                                   ref pixelX, ref pixelY,
                                   ref r, ref k, ref z,
                                   ref offsetX, ref offsetY);
                            Utils.SetColorCheckSrc(pSrc, ref width, ref height, ref offsetX, ref offsetY,
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
