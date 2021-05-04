using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Distorsions
{
    public class Swirl : IEffect
    {
		public float Degree { get; }
        public float Radius { get; }
        public float Twists { get; } = 1;
        public bool Antialiasing { get; set; } = true;

        public Swirl(float degree, float radius)
        {
            Degree = degree;
            Radius = radius;
        }

        public Swirl(float degree, float radius, float twists) : this(degree, radius)
        {
            Twists = twists;
        }

        public void Draw(SKBitmap bitmap)
        {
            void CalculateSwirl(ref float pixelX, ref float pixelY, ref float pixelDistance,
                ref float pixelAngle, ref float twistAngle, ref float swirlAmount, 
                ref float swirlX, ref float swirlY, ref float xOffset, ref float yOffset)
            {
                // compute the distance and angle from the swirl center:
                pixelDistance = MathF.Sqrt((pixelX * pixelX) + (pixelY * pixelY));
                pixelAngle = MathF.Atan2(pixelY, pixelX);

                // work out how much of a swirl to apply (1.0 in the center fading out to 0.0 at the radius):
                swirlAmount = 1.0f - (pixelDistance / Radius);
                if (swirlAmount > 0.0f)
                {
                    twistAngle = Twists * swirlAmount * MathF.PI * 2.0f;

                    // adjust the pixel angle and compute the adjusted pixel co-ordinates:
                    pixelAngle += twistAngle;
                    pixelX = MathF.Cos(pixelAngle) * pixelDistance;
                    pixelY = MathF.Sin(pixelAngle) * pixelDistance;
                }
                xOffset = swirlX + pixelX;
                yOffset = swirlY + pixelY;
            }

			int width = bitmap.Width;
			int height = bitmap.Height;	

			IntPtr pixelsAddr = bitmap.GetPixels();
			uint[,] buffer = new uint[width, height];

			unsafe
			{
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                float x0 = 0.5f * (width - 1), y0 = 0.5f * (height - 1),
                swirlX = height / 2.0f, swirlY = width / 2.0f,
                pixelX = 0, pixelY = 0, pixelDistance = 0, pixelAngle = 0, twistAngle = 0, swirlAmount = 0,
                offsetX = 0, offsetY = 0;

                // swirl with antialiasing
                if (Antialiasing)
                {
                    float fractionX = 0, fractionY = 0, oneMinusX = 0, oneMinusY = 0;
                    int ceilX = 0, ceilY = 0, floorX = 0, floorY = 0;

                    for (int x = 0; x < height; x++)
                    {
                        pixelX = x - swirlX;
                        for (int y = 0; y < width; y++)
                        {
                            pixelY = y - swirlY;
                            CalculateSwirl(ref pixelX, ref pixelY, ref pixelDistance,
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
                    for (int x = 0; x < height; x++)
                    {
                        pixelX = x - swirlX;
                        for (int y = 0; y < width; y++)
                        {
                            pixelY = y - swirlY;
                            CalculateSwirl(ref pixelX, ref pixelY, ref pixelDistance,
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
