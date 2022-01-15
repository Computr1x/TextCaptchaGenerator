using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DSVGPathPattern : BaseDrawable
    {
        public static readonly string square = "M -10 -10 L 10 -10, 10 10, -10 10 Z";
        public static readonly string triangle = "M 0 -10 L 0 -10, 10 10, -10 10 Z";
        public static readonly string rhombus = "M -10 0 L 0 -10, 10 0, 0 10 Z";
        public SKRect DrawingArea {get;set;}
        public string SVGPath {get;set;}
        public float Advance {get;set;} = 24f;
        public float Phase {get;set;} = 0f;

        public DSVGPathPattern(SKRect drawingArea, string sVGPath, SKPaint paint) : base(paint)
        {
            DrawingArea = drawingArea;
            SVGPath = sVGPath;
        }

        public override void Draw(SKCanvas canvas)
        {
            // SKPathEffect pathEffect = SKPathEffect.Create1DPath(SKPath.ParseSvgPathData(SVGPath), 50f, 10, SKPath1DPathEffectStyle.Translate);
            SKPathEffect pathEffect =
                SKPathEffect.Create2DPath(SKMatrix.CreateScale(25f,25f), 
                    SKPath.ParseSvgPathData(SVGPath)); 
            // TODO
            // https://docs.microsoft.com/en-us/xamarin/xamarin-forms/user-interface/graphics/skiasharp/curves/effects
            using (SKPath path = new SKPath())
            {
                path.AddRect(DrawingArea);
                Paint.PathEffect = pathEffect;

                canvas.Save();
                // canvas.ClipPath(path);
                // canvas.DrawRect(DrawingArea, Paint);
                canvas.DrawPath(path, Paint);
                canvas.Restore();

                // canvas.DrawPath(path, Paint);
            }
        }
    }
}