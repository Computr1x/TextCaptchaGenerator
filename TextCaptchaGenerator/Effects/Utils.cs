﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects
{
    public static class Utils
    {
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MakePixel(byte red, byte green, byte blue, byte alpha) =>
            (uint)((alpha << 24) | (blue << 16) | (green << 8) | red);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MultiplyFloatToPixel(float val, uint pixel) =>
            MakePixel((byte)((pixel >> 24) * val),
            (byte)((pixel >> 16 & 0xff) * val),
            (byte)((pixel >> 8 & 0xff) * val),
            (byte)((pixel & 0xff) * val));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetAntialisedColor(uint* pSrc, ref int width, ref int height, ref float offsetX, ref float offsetY, 
            ref uint[,] dst, ref int x, ref int y,
            ref int floorX, ref int floorY, ref int ceilX, ref int ceilY, 
            ref float fractionX, ref float fractionY, ref float oneMinusX, ref float oneMinusY)
        {
            floorX = (int)MathF.Floor(offsetX);
            floorY = (int)MathF.Floor(offsetY);
            ceilX = floorX + 1;
            ceilY = floorY + 1;
            fractionX = offsetX - floorX;
            fractionY = offsetY - floorY;
            oneMinusX = 1.0f - fractionX;
            oneMinusY = 1.0f - fractionY;

            // read and write the pixel
            if (floorX >= 0 && ceilX < width && floorY >= 0 && ceilY < height)
            {
                dst[y, x] = MultiplyFloatToPixel(oneMinusY, 
                        MultiplyFloatToPixel(oneMinusX, *(pSrc + floorY * width + floorX)) +
                        MultiplyFloatToPixel(fractionX, *(pSrc + floorY * width + ceilX))) + 
                    MultiplyFloatToPixel(fractionY, 
                        MultiplyFloatToPixel(oneMinusX, *(pSrc + ceilY * width + floorX)) +
                        MultiplyFloatToPixel(fractionX, *(pSrc + ceilY * width + ceilX)));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetColor(uint* pSrc, ref int width, ref int height, ref float offsetX, ref float offsetY,
            ref uint[,] dst, ref int x, ref int y)
        {
            if (offsetX >= 0 && offsetX < width && offsetY >= 0 && offsetY < height)
                dst[y, x] = *(pSrc + ((int)offsetY * width + (int)offsetX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetColor(uint* pSrc, ref int width, ref int height, ref uint[,] dst, ref int x, ref int y)
        {
            dst[y, x] = *(pSrc + (y * width + x));
        }
    }
}
