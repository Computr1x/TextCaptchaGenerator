using System;
using System.Text;
using System.Drawing.Text;
using System.Collections.Generic;
using SkiaSharp;
using TextCaptchaGenerator.Effects;
using TextCaptchaGenerator.RND.Range;

namespace TextCaptchaGenerator.RND{
    public class RNDManager {
        private Random rnd;
        public int Seed { get; private set;}

        //public record RNDIntRange(int low, int Max);
        // public record RNDSymbolsRange(char[] symbols);
        // public record RNDRectRange(float x, float y, float width, float height);
        // public record RNDRectRange(float x, float y, float width, float height);

        public RNDManager(int seed){
            Seed = seed;
            ResetRandom();
        }

        public void ResetRandom(){
            rnd = new Random(Seed);
        }

        public void ResetRandom(int seed){
            Seed = seed;
            ResetRandom();
        }

        public bool NextBool(){
            return rnd.NextSingle() >= 0.5f;
        }

        public float NextFloat(RNDBasicRange<float> range){
            int ceilPart = rnd.Next((int)range.Min, (int)range.Max);
            return ceilPart + rnd.NextSingle();
        }

        public float NextFloat(){
            return rnd.NextSingle();
        }

        public int NextInt(RNDBasicRange<int> range){
            return rnd.Next(range.Min, range.Max);
        }

        public int NextInt(int max){
            return rnd.Next(0, max);
        }

        public byte NextByte(RNDBasicRange<byte> range){
            return (byte)rnd.Next(range.Min, range.Max);
        }

        public byte NextByte(){
            return (byte)rnd.Next(0, 256);
        }
        

        public SKPoint NextSkPoint(RNDRectangle rect){
            return new SKPoint(
                NextFloat(new(rect.Left, rect.Right)),
                NextFloat(new(rect.Top, rect.Bottom)));
        }

        public SKPoint NextSkPoint(RNDCircle circle){
            return new SKPoint(
                circle.X + circle.Radius * MathF.Sqrt(rnd.NextSingle()),
                circle.Y + circle.Radius * MathF.Sqrt(rnd.NextSingle())
            );
        }

        public SKColor NextSKColor(byte alpha = 255){
            uint color = ((uint)rnd.Next());
            ColorUtils.SetAlphaChannel(ref color, alpha);
            return new SKColor(color);
        }

        public SKColor NextSKColor(RNDColorRange colorRange){
            return new SKColor(colorRange.Colors[rnd.Next(0, colorRange.Colors.Length)]);
        }

        
        public SKPaint NextSKFontPaint(RNDFontFamilyRange fontFamilyRange = null, 
        RNDBasicRange<int> fontSizeRange = null, 
        RNDColorRange colorRange = null){
            colorRange ??= new(3);
            fontSizeRange ??= new(24, 30);
            fontFamilyRange ??= new(new[]{"Arial"});

            SKColor color = colorRange.Colors[NextInt(colorRange.Colors.Length)];
            int fontSize = NextInt(fontSizeRange);
            string fontFamily = fontFamilyRange.FontFamilies[NextInt(fontFamilyRange.FontFamilies.Length)];

            var paint = new SKPaint();
            paint.TextSize = fontSize;
            paint.Color = color;
            paint.Typeface = SKTypeface.FromFamilyName(
                fontFamily, 
                SKFontStyleWeight.Normal, 
                SKFontStyleWidth.Normal, 
                SKFontStyleSlant.Italic);

            return paint;
        }

        public SKPaint NextSKPaint(RNDColorRange colorRange = null,
            bool isAntialias = true, 
            int strokeWidth = 2){
                SKColor color = NextSKColor();

                return new SKPaint() { Color=color, 
                IsAntialias = isAntialias, 
                StrokeWidth = strokeWidth};
        }       

        public string NextText(RNDTextRange textRange){
            int textLength = NextInt(textRange.TextLengthRange);

            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < textRange.Chars.Length; i++)
                sb.Append(textRange.Chars[rnd.Next(0, textRange.Chars.Length)]);

            return sb.ToString();
        }

        public T NextEnum<T>(RNDEnumRange<T> enumRange) where T : struct {
            return enumRange.EnumValues[NextInt(enumRange.EnumValues.Length)];
        }
    }
}