using SkiaSharp;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DEllipse : BaseDrawable
    {
        public SKPoint C { get; set; }
        public SKSize R { get; set; }

        public DEllipse(SKPoint c, SKSize r, SKPaint paint) : base(paint)
        {
            C = c;
            R = r;
        }

        public DEllipse(SKPoint c, float r, SKPaint paint) : base(paint)
        {
            C = c;
            R = new SKSize(r,r);
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawOval(C, R, Paint);
        }
    }
}
