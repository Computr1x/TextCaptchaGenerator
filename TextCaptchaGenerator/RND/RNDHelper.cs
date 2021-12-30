using System;
using System.Drawing.Text;
using System.Collections.Generic;
using SkiaSharp;
using TextCaptchaGenerator.Effects;

namespace TextCaptchaGenerator.RND {
    public class RNDHelper {
        private Random rnd;
        public int Seed { get; private set;}

        public record RNDIntRange(int low, int high);
        // public record RNDSymbolsRange(char[] symbols);
        // public record RNDRectRange(float x, float y, float width, float height);
        // public record RNDRectRange(float x, float y, float width, float height);

        public RNDHelper(int seed){
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

        public float NextFloat(float minValue, float maxValue){
            int ceilPart = rnd.Next((int)minValue, (int)maxValue);
            return ceilPart + rnd.NextSingle();
        }

        public float NextFloat(){
            return rnd.NextSingle();
        }

        public int NextInt(int minValue, int maxValue){
            return rnd.Next(minValue, maxValue);
        }

        public byte NextByte(){
            return (byte)rnd.Next(0, 256);
        }

        public SKPoint NextSkPoint(SKRect rect){
            return new SKPoint(
                NextFloat(rect.Left, rect.Right),
                NextFloat(rect.Top, rect.Bottom));
        }

        public SKPoint NextSkPoint(SKPoint center, float radius){
            return new SKPoint(
                center.X + radius * MathF.Sqrt(rnd.NextSingle()),
                center.Y + radius * MathF.Sqrt(rnd.NextSingle())
            );
        }

        public SKColor NextSKColor(){
            uint color = ((uint)rnd.Next() | 0xff000000);
            return new SKColor(color);
        }

        public SKColor NextSKColor(List<SKColor> colorList){
            return colorList[rnd.Next(0, colorList.Count)];
        }

        
        public SKPaint NextSKFontPaint(string fontFamily = "Arial", RNDIntRange fontSizeRange = null, SKColor? color = null){
            color ??= SKColors.Black;
            fontSizeRange ??= new(24, 30);

            var paint = new SKPaint();
            paint.TextSize = fontSize;
            paint.Color = color.Value;
            paint.Typeface = SKTypeface.FromFamilyName(
                fontFamily, 
                SKFontStyleWeight.Normal, 
                SKFontStyleWidth.Normal, 
                SKFontStyleSlant.Italic);

            return paint;
        }

        public SKPaint NextRandomSKFontPaint(RNDIntRange fontSizeRange = null, SKColor? color = null){
            if(fontList == null)
                InitListFonts();
            string fontName = fontList[rnd.Next(0, fontList.Length)];
            return NextSKFontPaint(fontName, fontSize, color);
        }

        

        public static NextText(){

        }
    }
}