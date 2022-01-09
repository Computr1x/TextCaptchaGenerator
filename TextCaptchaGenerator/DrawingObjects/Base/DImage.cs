using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.DrawingObjects.Base
{
    public class DImage : BaseDrawable
    {
        public int X { get; set; }
        public int Y { get; set; }
        public string Path { get; set; }

        public bool IgnoreIfNotExist { get; set; } = true;

        public DImage(int x, int y, string path): base(null)
        {
            X = x;
            Y = y;
            Path = path;
        }

        public override void Draw(SKCanvas canvas)
        {
            bool fileExist = File.Exists(Path);
            if (!fileExist && IgnoreIfNotExist)
                return;
            if (!fileExist)
                throw new FileNotFoundException($"Image on path {Path} not found");

            using var fsStream = File.OpenRead(Path);
            using var stream = new SKManagedStream(fsStream);
            using var bitmap = SKBitmap.Decode(stream);
            // draw the modified bitmap to the screen
            canvas.DrawBitmap(bitmap, X, Y);
        }
    }
}
