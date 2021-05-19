using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Transform
{
    public class Flip : IEffect
    {
        public enum eFlipType { Horizontal, Vertical, Both}

        public eFlipType FlipType { get; }

        public Flip(eFlipType flipType)
        {
            FlipType = flipType;
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

                if(FlipType == eFlipType.Horizontal)
                {
                    for (int x = 0, offsetX = width - 1; x < width; x++, offsetX--)
                        for (int y = 0; y < height; y++)
                            Utils.SetColor(pSrc, ref offsetX, ref width, ref height, ref y, ref buffer, ref x, ref y);
                }
                if (FlipType == eFlipType.Vertical)
                {
                    for (int x = 0; x < width; x++)
                        for (int y = 0, offsetY = height - 1; y < height; y++, offsetY--)
                            Utils.SetColor(pSrc, ref width, ref height, ref x, ref offsetY, ref buffer, ref x, ref y);
                }
                else
                {
                    for (int x = 0, offsetX = width - 1; x < width; x++, offsetX--)
                        for (int y = 0, offsetY = height - 1; y < height; y++, offsetY--)
                            Utils.SetColor(pSrc, ref width, ref height, ref offsetX, ref offsetY, ref buffer, ref x, ref y);
                }


                fixed (uint* newPtr = buffer)
                {
                    bitmap.SetPixels((IntPtr)newPtr);
                }
            }
        }
    }
}
