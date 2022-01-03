using System;

namespace TextCaptchaGenerator.RND.Range{
    public class RNDCircle {
        public float X {get;set;}
        public float Y {get;set;}
        public float Radius {get;set;}
        
        public RNDCircle(float x, float y, float radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }
    }
}