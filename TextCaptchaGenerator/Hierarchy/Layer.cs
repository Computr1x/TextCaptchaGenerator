using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.BaseObjects;
using TextCaptchaGenerator.DrawingObjects;

namespace TextCaptchaGenerator.Hierarchy
{
    public class Layer
    {
        protected SKImageInfo imageInfo;
        private SKBitmap bitmap;

        public SKBlendMode BlendMode { get; }
        public List<BaseDrawable> Drawables { get; }
        public List<IEffect> Effects { get; }
        public SKCanvas Canvas { get; }

        // TODO
        // add opacity

        
        public Layer(SKImageInfo imageInfo) : this(imageInfo, SKBlendMode.SrcOver, SKColors.Transparent) { }

        public Layer(SKImageInfo imageInfo, SKColor background) : this(imageInfo, SKBlendMode.SrcOver, background) { }

        public Layer(SKImageInfo imageInfo, SKBlendMode blendMode, SKColor background)
        {
            this.imageInfo = imageInfo;
            bitmap = new SKBitmap(imageInfo);
            Canvas = new SKCanvas(bitmap);
            Canvas.Clear(background);
            Drawables = new List<BaseDrawable>();
            Effects = new List<IEffect>();
            BlendMode = blendMode;
        }

        public SKBitmap DrawAll()
        {
            if (Drawables == null || Drawables.Count == 0)
                return null;

            foreach (var drawable in Drawables)
            {
                if (drawable.Effects == null || drawable.Effects.Count == 0)
                {
                    drawable.Draw(Canvas);
                }
                else
                {
                    var tempBitMap = new SKBitmap(imageInfo);
                    var tempCanvas = new SKCanvas(bitmap);
                    
                    drawable.Draw(tempCanvas);
                    
                    foreach(var effect in drawable.Effects)
                        effect.Draw(tempBitMap);

                    using (SKPaint paint = new SKPaint())
                    {
                        paint.BlendMode = BlendMode;
                        Canvas.DrawBitmap(tempBitMap, new SKPoint(0,0), paint);
                    }
                }
            }

            
            foreach (var effect in Effects)
                effect.Draw(bitmap);

            //using (SKPaint paint = new SKPaint())
            //{
            //    paint.BlendMode = BlendMode;
            //    Canvas.DrawBitmap(tempBitMap1, new SKPoint(0, 0), paint);
            //}

            return bitmap;
        }
    }
}
