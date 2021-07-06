using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;
using TextCaptchaGenerator.Effects.Color;

namespace TextCaptchaGenerator.Effects.Glitch
{
    // Floyd-Steinberg-Dithering
    public class FSDithering : IEffect
    {
        private static RGBColor[] _palete = new RGBColor[]
        {
            //new RGBColor(0,0,0),
            //new RGBColor(127,0,0),
            //new RGBColor(0,127,0),
            //new RGBColor(127,127,0),
            //new RGBColor(0,0,127),
            //new RGBColor(127,0,127),
            //new RGBColor(0,127,127),
            //new RGBColor(191,191,191),
            //new RGBColor(63,63,63),
            //new RGBColor(255,0,0),
            //new RGBColor(0,255,0),
            //new RGBColor(255,255,0),
            //new RGBColor(0,0,255),
            //new RGBColor(255,0,255),
            //new RGBColor(0,255,255),
            //new RGBColor(255,255,255)

            new RGBColor(0,0,0),
            new RGBColor(255,255,255),
            new RGBColor(0,255,0),
            new RGBColor(255,0,0),
            new RGBColor(0,0,255),
            new RGBColor(255,255,0),
            new RGBColor(255,0,255),
            new RGBColor(0,255,255)
        };

        public bool GrayScale { get; set; } = false;

        public void Draw(SKBitmap bitmap)
        {
            // normal distance
            float GetDistance(in byte r1, in byte g1, in byte b1, in byte r2, in byte g2, in byte b2)
            {
                // sqrt delete for optimization
                // MathF.Sqrt(MathF.Pow((float)(r2 - r1), 2f) + MathF.Pow((float)(g2 - g1), 2f) + MathF.Pow((float)(b2 - b1), 2f));
                return MathF.Pow((r2 - r1), 2f) + MathF.Pow((g2 - g1), 2f) + MathF.Pow((b2 - b1), 2f);
            }

            // weighted distance since eyes are most sensitive to green, and least sensitive to blue
            float GetWeightedDistance(in byte r1, in byte g1, in byte b1, in byte r2, in byte g2, in byte b2)
            {
                // sqrt delete for optimization
                return MathF.Pow((r2 - r1) * 0.30f, 2f) + MathF.Pow((g2 - g1) * 0.59f, 2f) + MathF.Pow((b2 - b1) * 0.11f, 2f);
            }

            void FindClosestPaleteColor(in byte r1, in byte g1, in byte b1, out byte r2, out byte g2, out byte b2)
            {
                int index = 0;
                float curD = 0, minD = float.MaxValue;

                for (int i = 0; i < _palete.Length; i++)
                {
                    curD = GetWeightedDistance(r1, g1, b1, _palete[i].R, _palete[i].G, _palete[i].B);
                    if (curD < minD)
                    {
                        minD = curD;
                        index = i;
                    }
                }

                r2 = _palete[index].R;
                g2 = _palete[index].G;
                b2 = _palete[index].B;
            }

            //byte Clip(int value)
            //{
            //    return value > 255 ? 255 : (byte)value;
            //}
            byte ClipF(float value)
            {
                return value > 255 ? 255 : (byte)value;
            }

            if (GrayScale)
            {
                GrayScale gs = new GrayScale();
                gs.Draw(bitmap);
            }


            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                fixed (uint* newPtr = buffer)
                {
                    uint* pSrc = (uint*)pixelsAddr.ToPointer();

                    int size = height * width * 4;
                    Buffer.MemoryCopy(pSrc, newPtr, size, size);

                    // if grayscale mode apply faster ditherig formula
                    if (GrayScale)
                    {
                        uint curColorRGB = 0;
                        byte a, gray, newGray, error;

                        for (int y = 0; y < height - 1; y++)
                        {
                            for (int x = 1; x < width - 1; x++)
                            {
                                curColorRGB = buffer[y, x];
                                a = (byte)(curColorRGB >> 24);
                                gray = (byte)(curColorRGB & 0xff);
                                newGray = gray > 128 ? 255 : 0;

                                buffer[y, x] = Utils.MakePixel(a, newGray, newGray, newGray);

                                // quant error
                                error = (byte)Math.Abs(gray - newGray);

                                // y, x + 1
                                curColorRGB = buffer[y, x + 1];
                                a = (byte)(curColorRGB >> 24);
                                gray = ClipF((curColorRGB & 0xff) + error * 7f / 16f);
                                buffer[y, x + 1] = Utils.MakePixel(a, gray, gray, gray);
                                // y + 1, x - 1
                                curColorRGB = buffer[y + 1, x - 1];
                                a = (byte)(curColorRGB >> 24);
                                gray = ClipF((curColorRGB & 0xff) + error * 3f / 16f);
                                buffer[y + 1, x - 1] = Utils.MakePixel(a, gray, gray, gray);
                                // y  + 1, x
                                curColorRGB = buffer[y + 1, x];
                                a = (byte)(curColorRGB >> 24);
                                gray = ClipF((curColorRGB & 0xff) + error * 5f / 16f);
                                buffer[y + 1, x] = Utils.MakePixel(a, gray, gray, gray);
                                // y + 1, x + 1
                                curColorRGB = buffer[y + 1, x + 1];
                                a = (byte)(curColorRGB >> 24);
                                gray = ClipF((curColorRGB & 0xff) + error * 1f / 16f);
                                buffer[y + 1, x + 1] = Utils.MakePixel(a, gray, gray, gray);
                            }
                        }
                    }
                    // rgb dithering
                    else
                    {
                        uint curColorRGB = 0;
                        byte a, r, g, b, r2 = 0, g2 = 0, b2 = 0;
                        // error for each channel
                        byte rE = 0, gE = 0, bE = 0;

                        for (int y = 0; y < height - 1; y++)
                        {
                            for (int x = 1; x < width - 1; x++)
                            {
                                curColorRGB = buffer[y, x];
                                ColorsConverter.UintToArgb(curColorRGB, out a, out r, out g, out b);

                                FindClosestPaleteColor(r, g, b, out r2, out g2, out b2);
                                buffer[y, x] = Utils.MakePixel(a, r2, g2, b2);

                                // quant error
                                rE = (byte)Math.Abs(r - r2);
                                gE = (byte)Math.Abs(g - g2);
                                bE = (byte)Math.Abs(b - b2);

                                // y, x + 1
                                ColorsConverter.UintToArgb(buffer[y, x + 1], out a, out r, out g, out b);
                                buffer[y, x + 1] = Utils.MakePixel(a, ClipF(r + rE * 7f / 16f), ClipF(g + gE * 7f / 16f), ClipF(b + bE * 7f / 16f));
                                // y + 1, x - 1
                                ColorsConverter.UintToArgb(buffer[y + 1, x - 1], out a, out r, out g, out b);
                                buffer[y + 1, x - 1] = Utils.MakePixel(a, ClipF(r + rE * 3f / 16f), ClipF(g + gE * 3f / 16f), ClipF(b + bE * 3f / 16f));
                                // y  + 1, x
                                ColorsConverter.UintToArgb(buffer[y + 1, x], out a, out r, out g, out b);
                                buffer[y + 1, x] = Utils.MakePixel(a, ClipF(r + rE * 5f / 16f), ClipF(g + gE * 5f / 16f), ClipF(b + bE * 5f / 16f));
                                // y + 1, x + 1
                                ColorsConverter.UintToArgb(buffer[y + 1, x + 1], out a, out r, out g, out b);
                                buffer[y + 1, x + 1] = Utils.MakePixel(a, ClipF(r + rE * 1f / 16f), ClipF(g + gE * 1f / 16f), ClipF(b + bE * 1f / 16f));
                            }
                        }
                    }

                    bitmap.SetPixels((IntPtr)newPtr);
                }
            }
        }
    }
}
