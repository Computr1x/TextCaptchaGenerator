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
            //if (Hue == 0 && Saturation == 0 && Brightness == 0)
            //    return;

            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                uint curColorRGB = 0;
                byte a, r, g, b;
                float rF, gF, bF, h, s, v;

                float tempVal = 0, hueF = Hue / 255f, satF = Saturation / 255f, briF = Brightness / 255f;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        curColorRGB = *(pSrc + y * width + x);
                        ColorsConverter.UintToArgb(curColorRGB, out a, out r, out g, out b);
                        if (a == 0)
                            continue;

                        rF = r / 255f;
                        gF = g / 255f;
                        bF = b / 255f;
                        ColorsConverter.RgbToHsb(rF, gF, bF, out h, out s, out v);

                        tempVal = h + hueF;
                        h = tempVal > 1f ? 1f : tempVal;
                        tempVal = s + satF;
                        s = tempVal > 1f ? 1f : tempVal;
                        tempVal = v + briF;
                        v = tempVal > 1f ? 1f : tempVal;

                        ColorsConverter.HsbToRgb(h, s, v, out rF, out gF, out bF);
                        r = (byte)(rF * 255);
                        g = (byte)(gF * 255);
                        b = (byte)(bF * 255);

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
