using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DRectangle : BaseDrawable
    {
        public SKRect Rect { get; set; }

        public DRectangle(SKRect rect, SKPaint paint) : base(paint)
        {
            Rect = rect;
        }

        public DRectangle(float xOffset, float yOffset, float width, float height, SKPaint paint) 
        : this(new SKRect(xOffset, yOffset, xOffset + width, yOffset + height), paint)
        {
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawRect(Rect, Paint);
        }
    }
}
