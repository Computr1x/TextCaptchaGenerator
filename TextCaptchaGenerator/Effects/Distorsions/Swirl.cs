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
		public double Degree { get; }

		public Swirl(double degree)
        {
			Degree = degree;
        }

        public void Draw(SKCanvas canvas, SKBitmap bitmap)
        {
			int width = bitmap.Width;
			int height = bitmap.Height;	


			IntPtr pixelsAddr = bitmap.GetPixels();

			uint[,] buffer = new uint[width, height];

			unsafe
			{
				//int cX = height / 2, cY = width / 2;
				//double theta, radius;
				//int newX, newY;

				uint* ptr = (uint*)pixelsAddr.ToPointer();

				double swirlX = height / 2.0, swirlY = width / 2.0, swirlRadius = 256, swirlTwists = 2;

				for (int x = 0; x < height; x++)
				{
					for (int y = 0; y < width; y++)
					{
						// compute the distance and angle from the swirl center:
						double pixelX = x - swirlX;
						double pixelY = y - swirlY;
						double pixelDistance = Math.Sqrt((pixelX * pixelX) + (pixelY * pixelY));
						double pixelAngle = Math.Atan2(pixelY, pixelX);

						// work out how much of a swirl to apply (1.0 in the center fading out to 0.0 at the radius):
						double swirlAmount = 1.0f - (pixelDistance / swirlRadius);
                        if (swirlAmount > 0.0f)
                        {
							double twistAngle = swirlTwists * swirlAmount * Math.PI * 2.0;

                            // adjust the pixel angle and compute the adjusted pixel co-ordinates:
                            pixelAngle += twistAngle;
                            pixelX = Math.Cos(pixelAngle) * pixelDistance;
                            pixelY = Math.Sin(pixelAngle) * pixelDistance;
                        }
						// read and write the pixel
						//dest.setPixel(x, y, src.getPixel(swirlX + pixelX, swirlY + pixelY));
						int newX = (int)Math.Round(swirlX + pixelX), newY = (int)Math.Round(swirlY + pixelY);

						if (!(newX < 0 || newX >= height || newY < 0 || newY >= width))
							buffer[newX, newY] = *ptr;
						ptr++;
					}
				}

				fixed (uint* newPtr = buffer)
				{
					bitmap.SetPixels((IntPtr)newPtr);
				}
			}

            //canvas = new SKCanvas(bitmap);
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		uint MakePixel(byte red, byte green, byte blue, byte alpha) =>
			(uint)((alpha << 24) | (blue << 16) | (green << 8) | red);
	}
}
