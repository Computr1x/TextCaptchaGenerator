using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.Hierarchy
{
    public class Image
    {
        public SKImageInfo info;
        public List<Layer> Layers { get; }


        public Image(int width, int height)
        {
            info = new SKImageInfo(width, height);
            Layers = new List<Layer>() { };
        }

        public SKImage DrawAll()
        {
            SKSurface surface = SKSurface.Create(info);
            SKCanvas canvas = surface.Canvas;

            SKPaint paint = new SKPaint();
            foreach (Layer layer in Layers)
            {
                paint.BlendMode = layer.BlendMode;
                canvas.DrawBitmap(layer.DrawAll(), new SKPoint(0, 0), paint);
            }

            return surface.Snapshot();
        }
    }
}
