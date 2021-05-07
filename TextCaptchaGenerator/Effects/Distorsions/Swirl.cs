using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Distorsions
{
    public class Swirl : IEffect
    {
        private float twists = 1;
        private bool customCoords = false;

        public float Degree { get; } = 0;
        public float Radius { get; }
        public float Twists { get => twists; }
        public bool Antialiasing { get; set; } = true;
        public bool Cloakwise { get; set; } = true;

        public int X { get; }
        public int Y { get; }

        public Swirl(float radius)
        {
            Radius = radius;
        }

        public Swirl(float radius, float twists) : this(radius)
        {
            this.twists = twists;
        }

        public Swirl(float radius, float twists, int x, int y) : this(radius, twists)
        {
            X = x;
            Y = y;
            customCoords = true;
        }

        public void Draw(SKBitmap bitmap)
        {
            void CalculateSwirl(ref int x, ref int y, ref float pixelX, ref float pixelY, ref float pixelDistance,
                 ref float pixelAngle, ref float twistAngle, ref float swirlAmount,
                 ref float swirlX, ref float swirlY, ref float offsetX, ref float offsetY)
            {
                pixelX = x - swirlX;
                pixelY = y - swirlY;

                // compute the distance and angle from the swirl center:
                pixelDistance = MathF.Sqrt(pixelX * pixelX + pixelY * pixelY);
                pixelAngle = MathF.Atan2(pixelY, pixelX);

                // work out how much of a swirl to apply (1.0 in the center fading out to 0.0 at the radius):
                swirlAmount = 1.0f - (pixelDistance / Radius);
                if (swirlAmount > 0.0f)
                {
                    twistAngle = Twists * swirlAmount * MathF.PI * 2.0f + Degree;

                    // adjust the pixel angle and compute the adjusted pixel co-ordinates:
                    pixelAngle += twistAngle + Degree;
                    pixelX = MathF.Cos(pixelAngle) * pixelDistance;
                    pixelY = MathF.Sin(pixelAngle) * pixelDistance;
                }
                offsetX = swirlX + pixelX;
                offsetY = swirlY + pixelY;
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            if (Cloakwise)
                twists = Math.Abs(twists);
            else
                twists = -Math.Abs(twists);

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[width, height];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                float swirlX = width / 2.0f, swirlY = height / 2.0f,
                pixelX = 0, pixelY = 0, pixelDistance = 0, pixelAngle = 0, twistAngle = 0, swirlAmount = 0,
                offsetX = 0, offsetY = 0;

                if (customCoords) {
                    swirlX = X;
                    swirlY = Y;
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
                            CalculateSwirl(ref x, ref y, ref pixelX, ref pixelY, ref pixelDistance,
                                ref pixelAngle, ref twistAngle, ref swirlAmount,
                                ref swirlX, ref swirlY, ref offsetX, ref offsetY);
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
                            CalculateSwirl(ref x, ref y, ref pixelX, ref pixelY, ref pixelDistance,
                                ref pixelAngle, ref twistAngle, ref swirlAmount,
                                ref swirlX, ref swirlY, ref offsetX, ref offsetY);
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
