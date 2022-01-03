using System;

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
    }
}