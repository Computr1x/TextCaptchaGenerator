using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DPolygon : BaseDrawable
    {
        public SKPoint[] Points { get; set; }
        public SKPathFillType FillType { get; }

        public DPolygon(SKPoint[] points, SKPaint paint) : base(paint)
        {
            Points = points;
            FillType = SKPathFillType.EvenOdd;
        }

        public DPolygon(SKPoint[] points, SKPathFillType fillType, SKPaint paint) : base(paint)
        {
            Points = points;
            FillType = fillType;
        }


        public override void Draw(SKCanvas canvas)
        {
            if (Points == null || Points.Length == 0)
                return;

            var path = new SKPath { FillType = FillType };
            path.MoveTo(Points.First());
            foreach (var t in Points)
                path.LineTo(t);

            path.LineTo(Points.First());
            path.Close();

            canvas.DrawPath(path, Paint);
        }
    }
}
