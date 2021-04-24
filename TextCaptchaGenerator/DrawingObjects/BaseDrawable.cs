using SkiaSharp;
using System.Collections.Generic;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.DrawingObjects
{
    public abstract class BaseDrawable : IDrawable
    {
        public SKPaint Paint { get; set; }

        public List<IDrawable> Effects { get; }

        public BaseDrawable(SKPaint paint)
        {
            Paint = paint;
            Effects = new List<IDrawable>();
        }

        public abstract void Draw(SKCanvas canvas);
    }
}
