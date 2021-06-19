using SkiaSharp;
using System;
using System.Linq;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Glitch
{
    public class Slices : IEffect
    {
        private Random _r;
        public int Count { get; set; } = 1;
        public int MinOffset { get; set; } = -10;
        public int MaxOffset { get; set; } = 20;

        public int SliceHeight { get; set; } = 4;

        public Slices(int seed = 0)
        {
            _r = new Random(seed);
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

                int[] sliceIndexes = Enumerable.Range(0, Count).Select(x => _r.Next(0, height)).ToArray();
                int[] sliceOffsets = Enumerable.Range(0, Count).Select(x => _r.Next(MinOffset, MaxOffset)).ToArray();

                fixed (uint* newPtr = buffer)
                {
                    int size = height * width * 4;
                    Buffer.MemoryCopy(pSrc, newPtr, size, size);

                    for (int i = 0; i < Count; i++)
                    {
                        for (int y = sliceIndexes[i]; y < sliceIndexes[i] + SliceHeight && y < height; y++)
                        {
                            if (sliceOffsets[i] >= 0)
                            {
                                for (int x = 0, offsetX = sliceOffsets[i]; x < width; x++, offsetX++)
                                {
                                    if (offsetX >= width)
                                        offsetX = 0;
                                    else if (offsetX < 0)
                                        offsetX = width + sliceOffsets[i];

                                    Utils.SetColor(pSrc, ref width, ref height, ref offsetX, ref y,
                                            ref buffer, ref x, ref y);
                                }
                            }
                            else
                            {
                                for (int x = width-1, offsetX = sliceOffsets[i]; x >= 0; x--, offsetX--)
                                {
                                    if (offsetX >= width)
                                        offsetX = 0;
                                    else if (offsetX < 0)
                                        offsetX = width + sliceOffsets[i];

                                    Utils.SetColor(pSrc, ref width, ref height, ref offsetX, ref y,
                                            ref buffer, ref x, ref y);
                                }
                            }
                        }
                    }

                    bitmap.SetPixels((IntPtr)newPtr);
                }
            }
        }
    }
}
