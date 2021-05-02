using SkiaSharp;

namespace TextCaptchaGenerator.BaseObjects
{
    public interface IEffect
    {
        public void Draw(SKCanvas canvas, SKBitmap bitmap);
    }
}
