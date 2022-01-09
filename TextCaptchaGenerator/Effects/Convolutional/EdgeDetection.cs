using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Convolutional
{
    public class EdgeDetection : IEffect
    {
        public float[,] Kernel { get; }
        public int Dim { get; }

        public EdgeDetection(float[,] kernel)
        {
            if (kernel.GetLength(0) != kernel.GetLength(1))
                throw new ArgumentException(
                    "Kernel must have equal dimensions. Provided dim:" +
                     $"{kernel.GetLength(0)}x{kernel.GetLength(1)}");
            if (kernel.GetLength(0) <= 0)
                throw new ArgumentException("Kernel must be bigger then 0");
            if (kernel.GetLength(0) % 2 == 0)
                throw new ArgumentException("Kernel must be odd value");
            Dim = kernel.GetLength(0);

            NormalizeKernel(kernel);
            Kernel = kernel;
        }

        private void NormalizeKernel(float[,] kernel)
        {
            float sum = 0;

            for (int i = 0; i < Dim; i++)
            {
                for (int j = 0; j < Dim; j++)
                {
                    sum += kernel[i, j];
                }
            }

            if (!(sum > 1)) return;
            {
                for (int i = 0; i < Dim; i++)
                {
                    for (int j = 0; j < Dim; j++)
                    {
                        kernel[i, j] /= sum;
                    }
                }
            }
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
                float r = 0, g = 0, b = 0; uint sourceColor = 0;
                int offsetX = 0, offsetY = 0;
                float kernelValue = 0;
                int radius = (int)MathF.Round(Dim / 2f) - 1;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        r = g = b = 0;

                        for (int i = -radius; i <= radius; i++)
                        {
                            offsetY = y + i;
                            if (offsetY < 0 || offsetY >= height)
                                continue;

                            for (int j = -radius; j <= radius; j++)
                            {
                                offsetX = x + j;
                                if (offsetX < 0 || offsetX >= width)
                                    continue;

                                kernelValue = Kernel[i + radius, j + radius];

                                sourceColor = *(pSrc + (offsetY * width + offsetX));
                                //if (x == 56 && y == 56)
                                //    Console.WriteLine($"OffX{offsetX}; OffY{offsetY}; KernI{i}; KernJ{j}");
                                r += ColorUtils.GetRedFChannel(in sourceColor) * kernelValue;
                                g += ColorUtils.GetGreenFChannel(in sourceColor) * kernelValue;
                                b += ColorUtils.GetBlueFChannel(in sourceColor) * kernelValue;
                            }
                        }

                        buffer[y, x] = ColorUtils.ArgbFToUint(
                            ColorUtils.GetAlphaFChannel(in sourceColor), in r, in g, in b);
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
