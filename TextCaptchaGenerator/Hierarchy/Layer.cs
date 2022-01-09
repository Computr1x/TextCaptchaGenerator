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
        protected SKRect imageRect;
        private SKBitmap bitmap;

        public SKBlendMode BlendMode { get; }
        public List<BaseDrawable> Drawables { get; }
        public List<IEffect> Effects { get; }
        public SKCanvas Canvas { get; }
        public byte Opacity {get;}
        
        public Layer(SKImageInfo imageInfo, byte opacity = 255) : this(imageInfo, SKBlendMode.SrcOver, SKColors.Transparent, opacity) { }

        public Layer(SKImageInfo imageInfo, SKColor background, byte opacity = 255) : this(imageInfo, SKBlendMode.SrcOver, background, opacity) { }

        public Layer(SKImageInfo imageInfo, SKBlendMode blendMode, SKColor background, byte opacity = 255)
        {
            this.imageInfo = imageInfo;
            bitmap = new SKBitmap(imageInfo);
            Canvas = new SKCanvas(bitmap);
            Canvas.Clear(background);
            Drawables = new List<BaseDrawable>();
            Effects = new List<IEffect>();
            BlendMode = blendMode;
            Opacity = opacity;
            imageRect = imageInfo.Rect;
        }

        public SKBitmap DrawAll()
        {
            if (Drawables == null || Drawables.Count == 0)
                return null;

            foreach (var drawable in Drawables)
            {
                if (drawable.Effects == null || drawable.Effects.Count == 0)
                {
                    drawable.Paint ??= new SKPaint();
                    drawable.Paint.Color = drawable.Paint.Color.WithAlpha(Opacity);
                    drawable.Draw(Canvas);
                }
                else
                {
                    SKBitmap tempBitMap = new(imageInfo);
                    SKCanvas tempCanvas = new(bitmap);
                    
                    drawable.Draw(tempCanvas);
                    
                    foreach(var effect in drawable.Effects)
                        effect.Draw(tempBitMap);

                    using SKPaint paint = new();
                    paint.Color = paint.Color.WithAlpha(Opacity);
                    paint.BlendMode = BlendMode;
                    Canvas.DrawBitmap(tempBitMap, new SKPoint(0,0), paint);
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
