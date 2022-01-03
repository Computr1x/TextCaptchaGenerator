using SkiaSharp;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DText : BaseDrawable
    {
        public SKPoint Point { get; set; }
        public string Text { get; set; }

        public DText(float x, float y, string text, SKPaint paint) : base(paint)
        {
            Point = new SKPoint(x, y);
            Text = text;
        }

        public DText(SKPoint p, string text,  SKPaint paint) : base(paint)
        {
            Point = p;
            Text = text;
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawText(Text, Point, Paint);
        }
    }
}
