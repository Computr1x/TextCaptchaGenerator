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
using TextCaptchaGenerator.BaseObjects;
using TextCaptchaGenerator.Effects.Glitch;
using TextCaptchaGenerator.Effects.Distort;
using TextCaptchaGenerator.Effects.Color;
using TextCaptchaGenerator.Effects.Noise;
using TextCaptchaGenerator.Effects.Transform;
using TextCaptchaGenerator.Effects.Convolutional;

namespace TextCaptchaGenerator.RND{
    public class RNDEffectManager {
        private RNDManager mgr;
        private RNDRectangle rect;
        private RNDEnumRange<eEffectType> allowedEffects;

        #region parameters
        public HsbCorrectionParsClass HsbCorrectionPars {get;set;}
        public float[,] EdgeDetectionKernel {get;set;}
        public BulgeParsClass BulgePars {get;set;}
        public RippleParsClass RipplePars {get;set;}
        // public SlitScanParsClass SlitScanPars {get;set;}
        public SwirlParsClass SwirlPars {get;set;}
        public WaveParsClass WavePars {get;set;}
        // public CrystalyzeParsClass CrystalyzePars {get;set;}
        // public FSDitheringParsClass FSDitheringPars {get;set;}
        public PixelateParsClass PixelatePars {get;set;}
        public RGBShiftParsClass RGBShiftPars {get;set;}
        // public SlicesParsClass SlicesPars {get;set;}
        public GaussNoiseParsClass GaussNoisePars {get;set;}
        // public PerlinNoiseParsClass PerlinNoisePars {get;set;}
        public FlipParsClass FlipPars {get;set;}
        public RotateParsClass RotatePars {get;set;}
        public ScaleParsClass ScalePars {get;set;}
        public ShiftParsClass ShiftPars {get;set;}
        public SkewParsClass SkewPars {get;set;}
        #endregion

        #region classes
        public record HsbCorrectionParsClass(
            RNDBasicRange<byte> HueRange,
            RNDBasicRange<byte> SaturationRange,
            RNDBasicRange<byte> BrightnesRange);

        public record BulgeParsClass(
            RNDRectangle DrawingArea, 
            RNDBasicRange<float> RadiusRange, 
            RNDBasicRange<float> StrenghtRange);

        // public record PolarCoordinatesParsClass ();
        public record RippleParsClass (
            RNDRectangle DrawingArea, 
            RNDBasicRange<float> RadiusRange, 
            RNDBasicRange<float> WaveRange, 
            RNDBasicRange<float> TraintRange);
        
        // public record SlitScanParsClass ();
        public record SwirlParsClass (
            RNDRectangle DrawingArea, 
            RNDBasicRange<float> RadiusRange, 
            RNDBasicRange<int> TwistsRange
        );
        public record WaveParsClass (
            RNDBasicRange<float> WaveRange,
            RNDBasicRange<float> AmplitudeRange,
            RNDEnumRange<Wave.eWaveType> WaveTypeRange
        );
        public record PixelateParsClass (
            RNDBasicRange<int> XBlockRange,
            RNDBasicRange<int> YBlockRange
        );
        public record RGBShiftParsClass (
            RNDBasicRange<int> XRedRange,
            RNDBasicRange<int> XGreenRange,
            RNDBasicRange<int> XBlueRange,
            RNDBasicRange<int> YRedRange,
            RNDBasicRange<int> YGreenRange,
            RNDBasicRange<int> YBlueRange
        );
        // public record SlicesParsClass ();
        public record GaussNoiseParsClass (
            RNDBasicRange<byte> AmountRange
        );
        // public record PerlinNoiseParsClass ();
        public record FlipParsClass (
            RNDEnumRange<Flip.eFlipType> FlipEnumRange
        );
        public record RotateParsClass (
            RNDBasicRange<float> DegreeRange
        );
        public record ScaleParsClass (
            RNDBasicRange<float> XScaleRange,
            RNDBasicRange<float> YScaleRange
        );
        public record ShiftParsClass (
            RNDBasicRange<int> XShiftRange,
            RNDBasicRange<int> YShiftRange
        );
        public record SkewParsClass (
            RNDBasicRange<float> LeftTopAngleRange,
            RNDBasicRange<float> LeftBottomAngleRange,
            RNDBasicRange<float> RightTopAngleRange,
            RNDBasicRange<float> RightBottomAngleRange
        );
        #endregion

        public RNDEffectManager(RNDRectangle rect, RNDManager manager, 
            RNDEnumRange<eEffectType> allowedEffects = null)
        {
            this.allowedEffects = allowedEffects ?? new();
            this.mgr = manager;
            this.rect = rect;
            InitDefaultParameters();
        }

        private void InitDefaultParameters(){
            HsbCorrectionPars = new HsbCorrectionParsClass(
                new RNDBasicRange<byte>(255),
                new RNDBasicRange<byte>(255),
                new RNDBasicRange<byte>(255)
            );

            EdgeDetectionKernel = new float[,]{
                {mgr.NextFloat(),mgr.NextFloat(),mgr.NextFloat()},
                {mgr.NextFloat(),mgr.NextFloat(),mgr.NextFloat()},
                {mgr.NextFloat(),mgr.NextFloat(),mgr.NextFloat()}
            };

            var radiusRange = new RNDBasicRange<float>(0, Math.Min(rect.Right, rect.Bottom) / 2);
            BulgePars = new BulgeParsClass
                (rect, radiusRange, 
                new RNDBasicRange<float>(0,3));

            RipplePars = new RippleParsClass(
                rect, radiusRange, new RNDBasicRange<float>(3), new RNDBasicRange<float>(1)
            );

            SwirlPars = new SwirlParsClass(
                rect, radiusRange, new RNDBasicRange<int>(1, 3)
            );

            WavePars = new WaveParsClass(
                new RNDBasicRange<float>(10),
                new RNDBasicRange<float>(3),
                new RNDEnumRange<Wave.eWaveType>()
            );

            PixelatePars = new PixelateParsClass(
                new RNDBasicRange<int>(0, Math.Min((int)rect.Right, 5)),
                new RNDBasicRange<int>(0, Math.Min((int)rect.Bottom, 5))
            );

            RGBShiftPars = new RGBShiftParsClass(
                new RNDBasicRange<int>(0, Math.Min((int)rect.Right, 10)),
                new RNDBasicRange<int>(0, Math.Min((int)rect.Right, 10)),
                new RNDBasicRange<int>(0, Math.Min((int)rect.Right, 10)),
                new RNDBasicRange<int>(0, Math.Min((int)rect.Bottom, 10)),
                new RNDBasicRange<int>(0, Math.Min((int)rect.Bottom, 10)),
                new RNDBasicRange<int>(0, Math.Min((int)rect.Bottom, 10))
            );

            GaussNoisePars = new GaussNoiseParsClass(
                new RNDBasicRange<byte>(255)
            );

            FlipPars = new FlipParsClass(
                new RNDEnumRange<Flip.eFlipType>()
            );

            RotatePars = new RotateParsClass(
                new RNDBasicRange<float>(-50, 50)
            );

            ScalePars = new ScaleParsClass(
                new RNDBasicRange<float>(0.5f, 2.0f),
                new RNDBasicRange<float>(0.5f, 2.0f)
            );

            ShiftPars = new ShiftParsClass(
                new RNDBasicRange<int>(0, Math.Min((int)rect.Right, 10)),
                new RNDBasicRange<int>(0, Math.Min((int)rect.Right, 10))
            );

            SkewPars = new SkewParsClass(
                new RNDBasicRange<float>(1f),
                new RNDBasicRange<float>(1f),
                new RNDBasicRange<float>(1f),
                new RNDBasicRange<float>(1f)
            );
        }

        public IEffect[] GetRandomEffects(int count){
            IEffect[] drawables = new IEffect[count];

            for(int i = 0; i < count; i++){
                drawables[i] = GetRandomEffectByType(
                    allowedEffects.EnumValues[
                        mgr.NextInt(allowedEffects.EnumValues.Length)]);
            }

            return drawables;
        }

        public IEffect GetRandomEffectByType(eEffectType effectType) => effectType switch{
            eEffectType.GrayScale=> GetRandomGrayScale(),
            eEffectType.HSBCorrection=> GetRandomHSBCorrection(),
            eEffectType.EdgeDetection=> GetRandomEdgeDetection(),
            eEffectType.Bulge=> GetRandomBulge(),
            eEffectType.PolarCoordinates=> GetRandomPolarCoordinates(),
            eEffectType.Ripple=> GetRandomRipple(),
            eEffectType.SlitScan=> GetRandomSlitScan(),
            eEffectType.Swirl=> GetRandomSwirl(),
            eEffectType.Wave=> GetRandomWave(),
            eEffectType.Crystalyze=> GetRandomCrystalyze(),
            eEffectType.FSDithering=> GetRandomFSDithering(),
            eEffectType.Pixelate=> GetRandomPixelate(),
            eEffectType.RGBShift=> GetRandomRGBShift(),
            eEffectType.Slices=> GetRandomSlices(),
            eEffectType.GaussNoise=> GetRandomGaussNoise(),
            eEffectType.PerlinNoise=> GetRandomPerlinNoise(),
            eEffectType.Flip=> GetRandomFlip(),
            eEffectType.Rotate=> GetRandomRotate(),
            eEffectType.Scale=> GetRandomScale(),
            eEffectType.Shift=> GetRandomShift(),
            eEffectType.Skew=> GetRandomSkew(),
            _ => throw new NotImplementedException($"Effect {effectType.ToString()} not implemented yet."),
        };

        #region random methods
        public IEffect GetRandomGrayScale(){
            return new GrayScale();
        }

        public IEffect GetRandomHSBCorrection(){
            return new HSBCorrection(
                mgr.NextByte(HsbCorrectionPars.HueRange),
                mgr.NextByte(HsbCorrectionPars.SaturationRange),
                mgr.NextByte(HsbCorrectionPars.BrightnesRange)
            );
        }

        public IEffect GetRandomEdgeDetection(){
            return new EdgeDetection(EdgeDetectionKernel);
        }

        public IEffect GetRandomBulge(){
            var point = mgr.NextSKPoint(BulgePars.DrawingArea);
            return new Bulge(
                (int)point.X, (int)point.Y, 
                mgr.NextFloat(BulgePars.RadiusRange),
                mgr.NextFloat(BulgePars.StrenghtRange)
            );
        }

        public IEffect GetRandomPolarCoordinates(){
            return new PolarCoordinates(PolarCoordinates.eConversionType.RectangularToPolar);
        }

        public IEffect GetRandomRipple(){
            var point = mgr.NextSKPoint(BulgePars.DrawingArea);
            return new Ripple(
                (int)point.X, (int)point.Y,
                mgr.NextFloat(RipplePars.RadiusRange),
                mgr.NextFloat(RipplePars.WaveRange),
                mgr.NextFloat(RipplePars.TraintRange));
        }

        public IEffect GetRandomSlitScan(){
            return new SlitScan(mgr.NextFloat(new RNDBasicRange<float>(1)));
        }

        public IEffect GetRandomSwirl(){
            var point = mgr.NextSKPoint(BulgePars.DrawingArea);

            return new Swirl(
                mgr.NextFloat(SwirlPars.RadiusRange), 
                mgr.NextInt(SwirlPars.TwistsRange),
                (int)point.X, (int)point.Y);
        }

        public IEffect GetRandomWave(){
            return new Wave(
                mgr.NextFloat(WavePars.WaveRange),
                mgr.NextFloat(WavePars.WaveRange),
                mgr.NextEnum<Wave.eWaveType>(WavePars.WaveTypeRange)
            );
        }

        public IEffect GetRandomCrystalyze(){
            return new Crystallize(mgr.Seed);
        }

        public IEffect GetRandomFSDithering(){
            return new FSDithering();
        }

        public IEffect GetRandomPixelate(){
            return new Pixelate(
                mgr.NextInt(PixelatePars.XBlockRange),
                mgr.NextInt(PixelatePars.YBlockRange)
            );
        }

        public IEffect GetRandomRGBShift(){
            return new RGBShift(
                mgr.NextInt(RGBShiftPars.XRedRange),
                mgr.NextInt(RGBShiftPars.XGreenRange),
                mgr.NextInt(RGBShiftPars.XBlueRange),
                mgr.NextInt(RGBShiftPars.YRedRange),
                mgr.NextInt(RGBShiftPars.YGreenRange),
                mgr.NextInt(RGBShiftPars.YBlueRange)
            );
        }

        public IEffect GetRandomSlices(){
            return new Slices(mgr.Seed);
        }

        public IEffect GetRandomGaussNoise(){
            return new GaussNoise(
                mgr.Seed, 
                mgr.NextByte(GaussNoisePars.AmountRange)
            );
        }

        public IEffect GetRandomPerlinNoise(){
            return new PerlinNoise(mgr.Seed);
        }

        public IEffect GetRandomFlip(){
            return new Flip(
                mgr.NextEnum<Flip.eFlipType>(FlipPars.FlipEnumRange));
        }

        public IEffect GetRandomRotate(){
            return new Rotate(
                mgr.NextFloat(RotatePars.DegreeRange)
            );
        }

        public IEffect GetRandomScale(){
            return new Scale(
                mgr.NextFloat(ScalePars.XScaleRange),
                mgr.NextFloat(ScalePars.YScaleRange)
            );
        }

        public IEffect GetRandomShift(){
            return new Shift(
                mgr.NextInt(ShiftPars.XShiftRange),
                mgr.NextInt(ShiftPars.YShiftRange)
            );
        }

        public IEffect GetRandomSkew(){
            return new Skew(
                mgr.NextFloat(SkewPars.LeftTopAngleRange),
                mgr.NextFloat(SkewPars.LeftBottomAngleRange),
                mgr.NextFloat(SkewPars.RightTopAngleRange),
                mgr.NextFloat(SkewPars.RightBottomAngleRange)
            );
        }
        #endregion
    }
}