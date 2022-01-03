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
            DEllipse ellipse = new(
                mgr.NextSkPoint(rect), 
                mgr.NextSKSize(new RNDBasicRange<float>(10, 30), 
                    new RNDBasicRange<float>(10, 30)), 
                mgr.NextSKPaint());
            return ellipse;
        }

        public DLine GetRandomLine(){
            SKPaint paint = mgr.NextSKPaint();

            DLine line = new DLine(
                mgr.NextSkPoint(rect),
                mgr.NextSkPoint(rect),
                paint
            );

            return line;
        }

        public DRectangle GetRandomRectangle(){
            float x = mgr.NextFloat(rect.Right), y = mgr.NextFloat(rect.Bottom),
            width = mgr.NextFloat(rect.Right / 2f), height = mgr.NextFloat(rect.Bottom / 2f);

            SKPaint paint = mgr.NextSKPaint();

            DRectangle rectangle = new(x, y, width, height, paint);
            return rectangle;
        }

        public DPolygon GetRandomPolygon(int pointsCount = 5){
            SKPoint[] points = new SKPoint[pointsCount];
            for(int i = 0; i < pointsCount; i++)
                points[i] = mgr.NextSkPoint(rect);

            SKPaint paint = mgr.NextSKPaint();

            DPolygon polygon = new(points, paint);

            return polygon;
        }

        public DText GetRandomText(){

            DText text = new(
                mgr.NextSkPoint(rect), 
                mgr.NextText(new RNDTextRange(RNDTextRange.asciiLetters, new(8))), 
                mgr.NextSKFontPaint());

            return text;
        }
    }
}