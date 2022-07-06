using System;
using SkiaSharp;

namespace TextCaptchaGenerator.RND.Range{
    public class RNDRectangle {
        public float Left {get;set;}
        public float Top {get;set;}
        public float Right {get;set;}
        public float Bottom {get;set;}

        public RNDRectangle(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public static explicit operator RNDRectangle(SKRect rect) => 
            new(rect.Left, rect.Top, rect.Right, rect.Bottom);
        public static explicit operator RNDRectangle(SKRectI rect) => 
            new(rect.Left, rect.Top, rect.Right, rect.Bottom);
        public static explicit operator RNDRectangle(SKImageInfo rect) => 
            new(0, 0, rect.Size.Width, rect.Size.Height);
    }
}