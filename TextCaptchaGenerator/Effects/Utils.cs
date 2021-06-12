using SkiaSharp;
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
        // SKIA SHARP store pixels as ARGB

        #region Make pixel
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MakePixel(byte alpha, byte red, byte green, byte blue) =>
            (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);
        #endregion

        #region Multiple pixel to float
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MultiplyFloatToPixel(float val, uint pixel) =>
            MakePixel(
                (byte)((pixel >> 24) * val),
                (byte)((pixel >> 16 & 0xff) * val),
                (byte)((pixel >> 8 & 0xff) * val),
                (byte)((pixel & 0xff) * val)
            );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MultiplyFloatToPixelWithoutAlpha(float val, uint pixel) =>
            MakePixel(
                (byte)(pixel >> 24),
                (byte)((pixel >> 16 & 0xff) * val),
                (byte)((pixel >> 8 & 0xff) * val),
                (byte)((pixel & 0xff) * val)
            );

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MultiplyPixelToPixel(uint pixel1, uint pixel2) =>
             MakePixel(
                (byte)((pixel1 >> 24) * (pixel2 >> 24) / 255),
                (byte)((pixel1 >> 16 & 0xff) * (pixel2 >> 16 & 0xff) / 255),
                (byte)((pixel1 >> 8 & 0xff) * (pixel2 >> 8 & 0xff) / 255),
                (byte)((pixel1 & 0xff) * (pixel2 & 0xff) / 255));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint MultiplyPixelToPixelWithoutAlpha(uint pixel1, uint pixel2) =>
            MakePixel(
                (byte)Math.Max(pixel1 >> 24, pixel2 >> 24),
                (byte)((pixel1 >> 16 & 0xff) * (pixel2 >> 16 & 0xff) / 255),
                (byte)((pixel1 >> 8 & 0xff) * (pixel2 >> 8 & 0xff) / 255),
                (byte)((pixel1 & 0xff) * (pixel2 & 0xff) / 255)
                );
        #endregion

        #region Boundaries check
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckBoundaries(ref int width, ref int height, ref float x, ref float y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckBoundaries(ref int width, ref int height, ref int x, ref int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
        #endregion

        #region Set color
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetColor(uint* pSrc, ref int width, ref int height, ref int offsetX, ref int offsetY,
            ref uint[,] dst, ref int x, ref int y)
        {
            dst[y, x] = *(pSrc + (offsetY * width + offsetX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetColor(uint* pSrc, ref int width, ref int height, ref float offsetX, ref float offsetY,
            ref uint[,] dst, ref int x, ref int y)
        {
            dst[y, x] = *(pSrc + ((int)offsetY * width + (int)offsetX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetColorCheckSrc(uint* pSrc, ref int width, ref int height, ref float offsetX, ref float offsetY,
            ref uint[,] dst, ref int x, ref int y)
        {
            if (CheckBoundaries(ref width, ref height, ref offsetX, ref offsetY))
                dst[y, x] = *(pSrc + ((int)offsetY * width + (int)offsetX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetColorCheckSrcDst(uint* pSrc, ref int width, ref int height, ref float offsetX, ref float offsetY,
            ref uint[,] dst, ref int x, ref int y)
        {
            if (CheckBoundaries(ref width, ref height, ref offsetX, ref offsetY) && CheckBoundaries(ref width, ref height, ref x, ref y))
                dst[y, x] = *(pSrc + ((int)offsetY * width + (int)offsetX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetColorCheckSrc(uint* pSrc, ref int width, ref int height, ref int offsetX, ref int offsetY,
            ref uint[,] dst, ref int x, ref int y)
        {
            if (CheckBoundaries(ref width, ref height, ref offsetX, ref offsetY))
                dst[y, x] = *(pSrc + (offsetY * width + offsetX));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public unsafe static void SetColor(uint* pSrc, ref int width, ref int height, ref uint[,] dst, ref int x, ref int y)
        {
            dst[y, x] = *(pSrc + (y * width + x));
        }

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
        #endregion

        #region Blend modes
        public delegate uint BlendMethod(uint foregrount, uint background);

        public static BlendMethod GetBlendFunction(SKBlendMode blendMode)
        {
            switch (blendMode)
            {
                case SKBlendMode.Clear:
                    return ClearBlend;
                case SKBlendMode.Src:
                    return SrcBlend;
                case SKBlendMode.Dst:
                    return DstBlend;
                case SKBlendMode.SrcOver:
                    return SrcOverBlend;
                case SKBlendMode.DstOver:
                    return DstOverBlend;
                //case SKBlendMode.SrcIn:
                //    break;
                //case SKBlendMode.DstIn:
                //    break;
                //case SKBlendMode.SrcOut:
                //    break;
                //case SKBlendMode.DstOut:
                //    break;
                //case SKBlendMode.SrcATop:
                //    break;
                //case SKBlendMode.DstATop:
                //    break;
                //case SKBlendMode.Xor:
                //    break;
                //case SKBlendMode.Plus:
                //    break;
                //case SKBlendMode.Modulate:
                //    break;
                //case SKBlendMode.Screen:
                //    break;
                //case SKBlendMode.Overlay:
                //    break;
                //case SKBlendMode.Darken:
                //    break;
                //case SKBlendMode.Lighten:
                //    break;
                //case SKBlendMode.ColorDodge:
                //    break;
                //case SKBlendMode.ColorBurn:
                //    break;
                //case SKBlendMode.HardLight:
                //    break;
                //case SKBlendMode.SoftLight:
                //    break;
                //case SKBlendMode.Difference:
                //    break;
                //case SKBlendMode.Exclusion:
                //    break;
                case SKBlendMode.Multiply:
                    return MultiplyBlend;
                //case SKBlendMode.Hue:
                //    break;
                //case SKBlendMode.Saturation:
                //    break;
                //case SKBlendMode.Color:
                //    break;
                //case SKBlendMode.Luminosity:
                //    break;
                default:
                    return ClearBlend;
            }
        }

        // Help methods
        public static byte CalcAlpha(uint src, uint dst)
        {
            float dstA = (byte)(dst >> 24 & 0xff) / 255f;
            float srcA = (byte)(src >> 24 & 0xff) / 255f;
            return (byte)((srcA + (dstA * (1 - srcA)))*255);
        }

        public static byte CalcAlphaByte(byte src, byte dst)
        {
            float dstA = dst / 255f;
            float srcA = src / 255f;
            return (byte)((srcA + (dstA * (1 - srcA)))*255);
        }
    

        public static uint BlendIgor(uint bg, uint fg)
        {
            byte
                bgA = (byte) (bg >> 24),
                fgA = (byte) (fg >> 24);

            if (fgA == 255 || bgA == 0)
            {
                return fg;
            }

            else if (fgA == 0)
            {
                return bg;
            }
            else
            {
                float bgM = bgA * (255 - fgA) / 255f;
                float reA = fgA + bgM;

                return MakePixel(
                    (byte)reA,
                    (byte)(((fg >> 16 & 0xff) * fgA + (bg >> 16 & 0xff) * bgM) / reA),
                    (byte)(((fg >> 8 & 0xff) * fgA + (bg >> 8 & 0xff) * bgM) / reA),
                    (byte)(((fg & 0xff) * fgA + (bg & 0xff) * bgM) / reA)
                );
            }
        }

        public static uint BlendPreMul(uint src, uint dst)
        {
            uint alpha = CalcAlpha(src, dst);
            uint rb = (src & 0xFF00FF) + ((alpha * (dst & 0xFF00FF)) >> 8);
            uint g = (src & 0x00FF00) + ((alpha * (dst & 0x00FF00)) >> 8);
            return (alpha << 24) | ((rb & 0xFF00FF) + (g & 0x00FF00));
        }

        public static uint BlendPreMulAlpha(uint src, uint dst, uint alpha)
        {
            uint rb = (src & 0xFF00FF) + ((alpha * (dst & 0xFF00FF)) >> 8);
            uint g = (src & 0x00FF00) + ((alpha * (dst & 0x00FF00)) >> 8);
            return (rb & 0xFF00FF) + (g & 0x00FF00);
        }

        //public static uint BlendTest(uint src, uint dst)
        //{
        //    byte dstA = (byte)(dst >> 24 & 0xff);
        //    byte srcA = (byte)(src >> 24 & 0xff);
        //    byte resA = (byte)(srcA + dstA * (255 - srcA));

        //    if (resA == 0)
        //        return 0;

        //    byte r = (byte)(((src >> 16 & 0xFF) * srcA + (dst >> 16 & 0xFF) * dstA * (255 - srcA)) / resA);
        //    byte g = (byte)(((src >> 8 & 0xFF) * srcA + (dst >> 8 & 0xFF) * dstA * (255 - srcA)) / resA);
        //    byte b = (byte)(((src & 0xFF) * srcA + (dst & 0xFF) * dstA * (255 - srcA)) / resA);

        //    return MakePixel(resA, r, g, b);
        //}

        public static uint AlphaBlendPixels(uint p1, uint p2)
        {
            uint AMASK = 0xFF000000;
            uint RBMASK = 0x00FF00FF;
            uint GMASK = 0x0000FF00;
            uint AGMASK = AMASK | GMASK;
            uint ONEALPHA = 0x01000000;
            uint a = (p2 & AMASK) >> 24;
            uint na = 255 - a;
            uint rb = ((na * (p1 & RBMASK)) + (a * (p2 & RBMASK))) >> 8;
            uint ag = (na * ((p1 & AGMASK) >> 8)) + (a * (ONEALPHA | ((p2 & GMASK) >> 8)));
            return ((rb & RBMASK) | (ag & AGMASK));
        }

        // Blending methods
        private static uint ClearBlend(uint src, uint dst)
        {
            return 0;
        }

        private static uint SrcBlend(uint src, uint dst)
        {
            return src;
        }

        private static uint DstBlend(uint src, uint dst)
        {
            return dst;
        }

        private static uint SrcOverBlend(uint src, uint dst)
        {
            byte srcA = (byte)(src >> 24 & 0xff);
            if (srcA == 255)
                return src;

            return BlendPreMul(dst, src);
        }

        private static uint DstOverBlend(uint src, uint dst)
        {
            byte dstA = (byte) (dst >> 24 & 0xff);
            if (dstA == 255)
                return dst;

            return BlendPreMul(src, dst);
        }


        private static uint MultiplyBlend(uint src, uint dst)
        {
            byte resA = CalcAlpha(src, dst);
            return MakePixel(
                resA,
                (byte)((src >> 16 & 0xff) * (dst >> 16 & 0xff) / 255),
                (byte)((src >> 8 & 0xff) * (dst >> 8 & 0xff) / 255),
                (byte)((src & 0xff) * (dst & 0xff) / 255));
        }


        #endregion




    }
}
