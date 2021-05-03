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
        public float Twists { get; }

        public Swirl(float degree, float radius)
        {
            Degree = degree;
            Radius = radius;
            Twists = 1;
        }

        public Swirl(float degree, float radius, float twists) : this(degree, radius)
        {
            Twists = twists;
        }

        public void Draw(SKBitmap bitmap)
        {
			int width = bitmap.Width;
			int height = bitmap.Height;	


			IntPtr pixelsAddr = bitmap.GetPixels();

			uint[,] buffer = new uint[width, height];

			unsafe
			{
                float x0 = 0.5f * (width - 1);
                float y0 = 0.5f * (height - 1);

                uint* ptr = (uint*)pixelsAddr.ToPointer();

                float swirlX = height / 2.0f, swirlY = width / 2.0f;
                float dx, dy, pixelDistance, pixelAngle, twistAngle, swirlAmount;

                for (int x = 0; x < height; x++)
                {
                    for (int y = 0; y < width; y++)
                    {
                        // compute the distance and angle from the swirl center:
                        dx = x - swirlX; dy = y - swirlY;
                        pixelDistance = MathF.Sqrt((dx * dx) + (dy * dy));
                        pixelAngle = MathF.Atan2(dy, dx);

                        // work out how much of a swirl to apply (1.0 in the center fading out to 0.0 at the radius):
                        swirlAmount = 1.0f - (pixelDistance / Radius);
                        if (swirlAmount > 0.0f)
                        {
                            twistAngle = Twists * swirlAmount * MathF.PI * 2.0f;

                            // adjust the pixel angle and compute the adjusted pixel co-ordinates:
                            pixelAngle += twistAngle;
                            dx = MathF.Cos(pixelAngle) * pixelDistance;
                            dy = MathF.Sin(pixelAngle) * pixelDistance;
                        }
                        // read and write the pixel
                        //dest.setPixel(x, y, src.getPixel(swirlX + pixelX, swirlY + pixelY));
                        int newX = (int)MathF.Floor(swirlX + dx), newY = (int)MathF.Floor(swirlY + dy);

                        if (!(newX < 0 || newX >= height || newY < 0 || newY >= width))
                            buffer[x, y] = *(ptr + (newY * width + newX));
                    }
                }

                fixed (uint* newPtr = buffer)
                {
                    bitmap.SetPixels((IntPtr)newPtr);
                }
			}
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		uint MakePixel(byte red, byte green, byte blue, byte alpha) =>
			(uint)((alpha << 24) | (blue << 16) | (green << 8) | red);


        //		float swirlX = height / 2.0, swirlY = width / 2.0, swirlRadius = 256, swirlTwists = 2;

        //				for (int x = 0; x<height; x++)
        //				{
        //					for (int y = 0; y<width; y++)
        //					{
        //						// compute the distance and angle from the swirl center:
        //						float pixelX = x - swirlX;
        //		float pixelY = y - swirlY;
        //		float pixelDistance = MathF.Sqrt((pixelX * pixelX) + (pixelY * pixelY));
        //		float pixelAngle = MathF.Atan2(pixelY, pixelX);

        //		// work out how much of a swirl to apply (1.0 in the center fading out to 0.0 at the radius):
        //		float swirlAmount = 1.0f - (pixelDistance / swirlRadius);
        //                        if (swirlAmount > 0.0f)
        //                        {
        //							float twistAngle = swirlTwists * swirlAmount * MathF.PI * 2.0;

        //		// adjust the pixel angle and compute the adjusted pixel co-ordinates:
        //		pixelAngle += twistAngle;
        //                            pixelX = MathF.Cos(pixelAngle) * pixelDistance;
        //		pixelY = MathF.Sin(pixelAngle) * pixelDistance;
        //	}
        //	// read and write the pixel
        //	//dest.setPixel(x, y, src.getPixel(swirlX + pixelX, swirlY + pixelY));
        //	int newX = (int)MathF.Floor(swirlX + pixelX), newY = (int)MathF.Floor(swirlY + pixelY);

        //						if (!(newX< 0 || newX >= height || newY< 0 || newY >= width))
        //							buffer[newX, newY] = *ptr;
        //						ptr++;
        //					}
        //				}

        //				fixed (uint* newPtr = buffer)
        //{
        //	bitmap.SetPixels((IntPtr)newPtr);
        //}
    }
}
