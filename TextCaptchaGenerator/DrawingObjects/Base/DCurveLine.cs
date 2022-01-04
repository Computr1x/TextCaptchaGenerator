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
            for (int i = 0; i < Points.Length; i++)
                path.LineTo(Points[i]);
            path.Close();
            path.
            canvas.DrawPath(path, Paint);
        }
    }
}