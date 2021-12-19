using SkiaSharp;
using System.Collections.Generic;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.DrawingObjects
{
    public abstract class BaseDrawable : IDrawable
    {
        public SKPaint Paint { get; set; }

        public List<IEffect> Effects { get; }
        

        public BaseDrawable(SKPaint paint)
        {
            Paint = paint;
            Effects = new List<IEffect>();
        }

        // public BaseDrawable(SKPaint paint, SKShader shader)
        // {
        //     paint.Shader = shader;
        //     Paint = paint;
        //     Effects = new List<IEffect>();
        // }

        public abstract void Draw(SKCanvas canvas);
    }
}
