using SkiaSharp;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DText : BaseDrawable
    {
        public SKPoint P { get; set; }
        public string Text { get; set; }
        public SKFont Font { get; set; }


        public DText(float x, float y, string text, SKFont font, SKPaint paint) : base(paint)
        {
            Font = font;
            P = new SKPoint(x, y);
            Text = text;
        }

        public DText(SKPoint p, string text, SKFont font, SKPaint paint) : base(paint)
        {
            Font = font;
            P = p;
            Text = text;
        }

        public override void Draw(SKCanvas canvas)
        {
            canvas.DrawText(Text, P, Paint);
        }
    }
}
