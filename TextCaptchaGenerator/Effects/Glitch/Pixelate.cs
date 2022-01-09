using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Glitch
{
    public class Pixelate : IEffect
    {
        private int _xBlockSize, _yBlockSize;

        public Pixelate(int xBlockSize, int yBlockSize)
        {
            _xBlockSize = xBlockSize;
            _yBlockSize = yBlockSize;
        }

        public void Draw(SKBitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                uint curPixel = 0, a = 0, r = 0, g = 0, b = 0, count = 0;
                int curXLength = 0, curYLength = 0; ;

                for (int y = 0; y < height; y+=_yBlockSize)
                {
                    curYLength = (y + _yBlockSize < height) ? y + _yBlockSize : height - y;
                    for (int x = 0; x < width; x+=_xBlockSize)
                    {
                        curXLength = (x + _xBlockSize < width) ? x + _xBlockSize : width - x;
                        count = a = r = g = b = 0;

                        for(int yy = y; yy < curYLength; yy++)
                        {
                            for (int xx = x; xx < curXLength; xx++)
                            {
                                curPixel = *(pSrc + yy * width + xx);

                                if (curPixel >> 24 <= 0) continue;
                                count++;
                                a += curPixel >> 24;
                                r += curPixel >> 16 & 0xff;
                                g += curPixel >> 8 & 0xff;
                                b += curPixel & 0xff;
                            }
                        }

                        if (count <= 0) continue;
                        a /= count;
                        r /= count;
                        g /= count;
                        b /= count;

                        curPixel = Utils.MakePixel((byte)a, (byte)r, (byte)g, (byte)b);

                        for(int i = y; i < curYLength; i++)
                        {
                            for (int j = x; j < curXLength; j++)
                            {
                                buffer[i, j] = curPixel;
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
