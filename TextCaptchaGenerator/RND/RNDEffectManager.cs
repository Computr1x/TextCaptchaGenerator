using System;
using System.Text;
using System.Drawing.Text;
using System.Collections.Generic;
using SkiaSharp;
using TextCaptchaGenerator.Effects;
using TextCaptchaGenerator.RND.Range;
using TextCaptchaGenerator.RND;
using TextCaptchaGenerator.BaseObjects.Enums;
using TextCaptchaGenerator.DrawingObjects;
using TextCaptchaGenerator.DrawingObjects.Base;

namespace TextCaptchaGenerator.RND{
    public class RNDEffectManager {
        private RNDManager mgr;
        private RNDRectangle rect;
        private RNDEnumRange<eDrawableType> allowedEffects;

        public EllipseParsClass EllipsePars {get;set;}
        public LineParsClass LinePars {get;set;}
        public RectangleParsClass RectanglePars {get;set;}
        public PolygonParsClass PolygonPars {get;set;}
        public TextParsClass TextPars {get;set;}

        public record EllipseParsClass(
            RNDRectangle DrawingArea, 
            RNDSizeRange<float> SizeRange, 
            RNDColorRange ColorRange);

        public record LineParsClass(
            RNDRectangle DrawingArea, 
            RNDBasicRange<int> StrokeWidthRange, 
            RNDColorRange ColorRange);

        public record RectangleParsClass(
            RNDRectangle DrawingArea, 
            RNDBasicRange<int> StrokeWidthRange, 
            RNDColorRange ColorRange,
            bool isFilled = true);

        public record PolygonParsClass(
            RNDRectangle DrawingArea, 
            RNDBasicRange<int> PointsCountRange,
            RNDBasicRange<int> StrokeWidthRange, 
            RNDColorRange ColorRange,
            bool isFilled = true);

        public record TextParsClass(
            RNDRectangle DrawingArea, 
            RNDFontFamilyRange FontFamilyRange,
            RNDTextRange TextRange,
            RNDBasicRange<int> FontSizeRange,
            RNDColorRange ColorRange,
            bool isCenetered = false);

        public RNDEffectManager(RNDRectangle rect, RNDManager manager, 
            RNDEnumRange<eDrawableType> allowedEffects = null)
        {
            this.allowedEffects = allowedEffects ?? new();
            mgr = manager;
            this.rect = rect;
        }

        // public BaseDrawable[] GetRandomEffects(int count){
        //     BaseDrawable[] drawables = new BaseDrawable[count];

        //     for(int i = 0; i < count; i++){
        //         drawables[i] = GetRandomEffectByType(
        //             allowedEffects.EnumValues[
        //                 mgr.NextInt(allowedEffects.EnumValues.Length)]);
        //     }

        //     return drawables;
        // }

        // public BaseDrawable GetRandomEffectByType(eEffectType effectType) => effectType switch{
        //     eEffectType.GrayScale=> GetRandomGrayScale(),
        //     eEffectType.HSBCorrection=> GetRandomHSBCorrection(),
        //     eEffectType.EdgeDetection=> GetRandomEdgeDetection(),
        //     eEffectType.Bulge=> GetRandomBulge(),
        //     eEffectType.PolarCoordinates=> GetRandomPolarCoordinates(),
        //     eEffectType.Ripple=> GetRandomRipple(),
        //     eEffectType.SlitScan=> GetRandomSlitScan(),
        //     eEffectType.Swirl=> GetRandomSwirl(),
        //     eEffectType.Wave=> GetRandomWave(),
        //     eEffectType.Crystalyze=> GetRandomCrystalyze(),
        //     eEffectType.FSDithering=> GetRandomFSDithering(),
        //     eEffectType.Pixelate=> GetRandomPixelate(),
        //     eEffectType.RGBShift=> GetRandomRGBShift(),
        //     eEffectType.Slices=> GetRandomSlices(),
        //     eEffectType.GaussNoise=> GetRandomGaussNoise(),
        //     eEffectType.PerlinNoise=> GetRandomPerlinNoise(),
        //     eEffectType.Flip=> GetRandomFlip(),
        //     eEffectType.Rotate=> GetRandomRotate(),
        //     eEffectType.Scale=> GetRandomScale(),
        //     eEffectType.Shift=> GetRandomShift(),
        //     eEffectType.Skew=> GetRandomSkew(),
        //     _ => throw new NotImplementedException($"Effect {effectType.ToString()} not implemented yet."),
        // };
    }
}