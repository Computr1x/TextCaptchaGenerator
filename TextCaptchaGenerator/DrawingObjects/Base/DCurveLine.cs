using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DCurveLine : BaseDrawable
    {
        public SKPoint[] Points { get; set; }

        public DCurveLine(SKPoint[] points, SKPaint paint) : base(paint)
        {
            Points = points;
        }

        public override void Draw(SKCanvas canvas)
        {
            if (Points == null || Points.Length == 0)
                return;

            var path = new SKPath();
            path.MoveTo(Points.First());
            for (int i = 1; i < Points.Length; i++){
                var p1 = Points[i-1];
                var p2 = Points[i];
                path.ArcTo(p1, p2, MathF.Sqrt(MathF.Pow(p2.X - p1.X, 2f)+MathF.Pow(p2.Y - p1.Y, 2f)));
            }
            // path.Close();
            canvas.DrawPath(path, Paint);
        }
    }
}