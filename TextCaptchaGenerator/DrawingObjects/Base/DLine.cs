using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DLine : BaseDrawable
    {
        public SKPoint P1 { get; set; }
        public SKPoint P2 { get; set; }

        public DLine(SKPoint p1, SKPoint p2, SKPaint paint) : base(paint)
        {
            P1 = p1;
            P2 = p2;
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawLine(P1, P2, Paint);
        }
    }
}
