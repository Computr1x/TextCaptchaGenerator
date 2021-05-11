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
            for (int i = 0; i < Points.Length; i++)
                path.LineTo(Points[i]);
            path.LineTo(Points.First());
            path.Close();

            canvas.DrawPath(path, Paint);
        }
    }
}
