using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Glitch
{
    public class RGBShift : IEffect
    {
        int redXOffset, greenXOffset, blueXOffset, redYOffset, greenYOffset, blueYOffset;

        public int BlueYOffset { get => blueYOffset; set => blueYOffset = value; }
        public int GreenYOffset { get => greenYOffset; set => greenYOffset = value; }
        public int RedYOffset { get => redYOffset; set => redYOffset = value; }
        public int BlueXOffset { get => blueXOffset; set => blueXOffset = value; }
        public int GreenXOffset { get => greenXOffset; set => greenXOffset = value; }
        public int RedXOffset { get => redXOffset; set => redXOffset = value; }

        public RGBShift(int redXOffset, int greenXOffset, int blueXOffset, int redYOffset, int greenYOffset, int blueYOffset)
        {
            RedXOffset = redXOffset;
            GreenXOffset = greenXOffset;
            BlueXOffset = blueXOffset;
            RedYOffset = redYOffset;
            GreenYOffset = greenYOffset;
            BlueYOffset = blueYOffset;
        }

        public RGBShift(int offset)
        {
            RedXOffset = RedYOffset = offset;
            GreenXOffset = GreenYOffset = -offset;
            BlueXOffset = offset;
            BlueYOffset = -offset;
            //RedXOffset = -3;
            //BlueYOffset = 6;
        }

        enum eResChannel { Red, Green, Blue };

        public void Draw(SKBitmap bitmap)
        {
            unsafe void RGBShiftMethod(uint* pSrc, ref int x, ref int y, ref int curXOffset, ref int curYOffset, ref int xOffset, ref int yOffset, 
                ref int width, ref int height, ref uint sourceColor, ref uint resColor, eResChannel resChannel)
            {
                curXOffset = x + xOffset;
                curYOffset = y + yOffset;

                if (curXOffset >= width)
                    curXOffset -= width;
                else if (curXOffset < 0)
                    curXOffset += width;
                if (curYOffset >= height)
                    curYOffset -= height;
                else if (curYOffset < 0)
                    curYOffset += height;

                sourceColor = *(pSrc + (curYOffset * width + curXOffset));

                switch (resChannel)
                {
                    case eResChannel.Red:
                        resColor = (sourceColor & 0xffff0000) /*| (targetColor & 0x0000ffff)*/;
                        break;
                    case eResChannel.Green:
                        resColor = (sourceColor & 0xff00ff00) /*| (targetColor & 0x00ff00ff)*/;
                        break;
                    case eResChannel.Blue:
                        resColor = (sourceColor & 0xff0000ff) /*| (targetColor & 0x00ffff00)*/;
                        break;
                    default:
                        break;
                }
            }

            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();
                uint r = 0, g = 0, b = 0, sourceColor = 0;
                int curXOffset = 0, curYOffset = 0;

                //eResChannel resChannel = eResChannel.Blue;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {

                        //r = g = b = targetColor = sourceColor = 0;

                        // red shift
                        RGBShiftMethod(pSrc, ref x, ref y, ref curXOffset, ref curYOffset, ref redXOffset, ref redYOffset, 
                            ref width, ref height, ref sourceColor, ref r, eResChannel.Red);
                        // green shift
                        RGBShiftMethod(pSrc, ref x, ref y, ref curXOffset, ref curYOffset, ref greenXOffset, ref greenYOffset,
                            ref width, ref height, ref sourceColor, ref g, eResChannel.Green);
                        // blue shift
                        RGBShiftMethod(pSrc, ref x, ref y, ref curXOffset, ref curYOffset, ref blueXOffset, ref blueYOffset,
                            ref width, ref height, ref sourceColor, ref b, eResChannel.Blue);

                        buffer[y, x] = (r | g | b);
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
