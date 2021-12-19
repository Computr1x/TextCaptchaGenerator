using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Color
{
    public class HSBCorrection : IEffect
    {
        private byte hue, saturation, brightness;

        public byte Hue
        {
            get => hue;
            set { hue = (byte)(value % 256); }
        }

        public byte Saturation
        {
            get => saturation;
            set { saturation = (byte)(value % 256); }
        }

        public byte Brightness
        {
            get => brightness;
            set { brightness = (byte)(value % 256); }
        }

        public HSBCorrection()
        {
        }

        public HSBCorrection(byte hue, byte saturation, byte brightness)
        {
            Hue = hue;
            Saturation = saturation;
            Brightness = brightness;
        }

        public void Draw(SKBitmap bitmap)
        {
            if (Hue == 0 && Saturation == 0 && Brightness == 0)
                return;

            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                uint curColorRGB = 0;
                byte a, r, g, b, h, s, v;
                // addititonal fields for ref optimization
                byte rgbMin = 0, rgbMax = 0, region = 0, remainder = 0, p = 0, q = 0, t = 0;
                int tempVal = 0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        curColorRGB = *(pSrc + y * width + x);
                        ColorUtils.UintToArgb(curColorRGB, out a, out r, out g, out b);
                        if (a == 0)
                            continue;

                        ColorsConverter.RgbToHsb(r, g, b, ref rgbMin, ref rgbMax, out h, out s, out v);

                        tempVal = h + Hue;
                        h = (byte)(tempVal > 255 ? 255 : tempVal);
                        tempVal = s + Saturation;
                        s = (byte)(tempVal > 255 ? 255 : tempVal);
                        tempVal = v + Brightness;
                        v = (byte)(tempVal > 255 ? 255 : tempVal);

                        ColorsConverter.HsbToRgb(h, s, v, ref region, ref remainder, ref p, ref q, ref t, out r, out g, out b);

                        buffer[y, x] = Utils.MakePixel(a, r, g, b);
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
