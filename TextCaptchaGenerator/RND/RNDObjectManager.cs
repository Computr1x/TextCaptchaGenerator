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
    public class RNDObjectManager {
        private RNDManager mgr;
        private RNDRectangle rect;
        private RNDEnumRange<eDrawableType> allowedObjects;

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

        public RNDObjectManager(RNDRectangle rect, RNDManager manager, 
            RNDEnumRange<eDrawableType> allowedObjects = null)
        {
            this.allowedObjects = allowedObjects ?? new();
            mgr = manager;
            this.rect = rect;
        }

        public BaseDrawable[] GetRandomDrawable(int count){
            BaseDrawable[] drawables = new BaseDrawable[count];

            for(int i = 0; i < count; i++){
                drawables[i] = GetRandomDrawableByType(
                    allowedObjects.EnumValues[
                        mgr.NextInt(allowedObjects.EnumValues.Length)]);
            }

            return drawables;
        }

        public BaseDrawable GetRandomDrawableByType(eDrawableType drawableType) => drawableType switch{
            eDrawableType.Ellipse => GetRandomEllipse(),
            eDrawableType.Line => GetRandomLine(),
            eDrawableType.Rectangle => GetRandomRectangle(),
            eDrawableType.Polygon => GetRandomPolygon(),
            eDrawableType.Text => GetRandomText(),
            _ => throw new NotImplementedException($"Type {drawableType.ToString()} not implemented yet."),
        };

        public DEllipse GetRandomEllipse(){
            EllipsePars ??= new EllipseParsClass(
                rect,
                new RNDSizeRange<float>(
                    new RNDBasicRange<float>(10, 30), 
                    new RNDBasicRange<float>(10, 30)),
                new RNDColorRange(255));

            DEllipse ellipse = new(
                mgr.NextSkPoint(EllipsePars.DrawingArea), 
                mgr.NextSKSize(EllipsePars.SizeRange), 
                mgr.NextSKPaint(EllipsePars.ColorRange));
            return ellipse;
        }

        public DLine GetRandomLine(){
            LinePars ??= new LineParsClass(
                rect,
                new RNDBasicRange<int>(3, 30),
                new RNDColorRange(25));

            DLine line = new DLine(
                mgr.NextSkPoint(LinePars.DrawingArea),
                mgr.NextSkPoint(LinePars.DrawingArea),
                mgr.NextSKPaint(LinePars.ColorRange, true, mgr.NextInt(LinePars.StrokeWidthRange))
            );

            return line;
        }

        public DRectangle GetRandomRectangle(){
            RectanglePars ??= new RectangleParsClass(
                rect,
                new RNDBasicRange<int>(2, 32),
                new RNDColorRange(32),
                mgr.NextBool()
            );

            DRectangle rectangle = new DRectangle(
                mgr.NextFloat(RectanglePars.DrawingArea.Right),
                mgr.NextFloat(RectanglePars.DrawingArea.Bottom),
                mgr.NextFloat(RectanglePars.DrawingArea.Right / 2f),
                mgr.NextFloat(RectanglePars.DrawingArea.Right / 2f),
                mgr.NextSKPaint(RectanglePars.ColorRange, true, mgr.NextInt(RectanglePars.StrokeWidthRange)));
            return rectangle;
        }

        public DPolygon GetRandomPolygon(){
            PolygonPars ??= new PolygonParsClass(
                rect,
                new RNDBasicRange<int>(5, 8),
                new RNDBasicRange<int>(2, 32),
                new RNDColorRange(32),
                mgr.NextBool()
            );

            int pointsCount = mgr.NextInt(PolygonPars.PointsCountRange);
            SKPoint[] points = new SKPoint[pointsCount];
            for(int i = 0; i < pointsCount; i++)
                points[i] = mgr.NextSkPoint(rect);

            DPolygon polygon = new DPolygon(
                points, 
                mgr.NextSKPaint(
                    PolygonPars.ColorRange, 
                    true, 
                    mgr.NextInt(PolygonPars.StrokeWidthRange))
                );

            return polygon;
        }

        public DText GetRandomText(){
            TextPars ??= new TextParsClass(
                rect,
                new RNDFontFamilyRange(),
                new RNDTextRange(
                    RNDTextRange.asciiUpperCase,
                    new RNDBasicRange<int>(8)),
                new RNDBasicRange<int>(14, 32),
                new RNDColorRange(32),
                true
            );
                
            DText text = new DText(
                (TextPars.isCenetered ? 
                new SKPoint(rect.Right / 2f, rect.Bottom / 2f) :
                mgr.NextSkPoint(rect)),
                mgr.NextText(TextPars.TextRange), 
                mgr.NextSKFontPaint(
                    TextPars.FontFamilyRange,
                    TextPars.FontSizeRange,
                    TextPars.ColorRange));

            return text;
        }
    }
}