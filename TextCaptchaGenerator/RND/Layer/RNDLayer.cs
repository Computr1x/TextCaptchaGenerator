using System;
using System.Text;
using System.Collections.Generic;
using SkiaSharp;
using TextCaptchaGenerator.Effects;
using TextCaptchaGenerator.RND;
using TextCaptchaGenerator.RND.Range;
using TextCaptchaGenerator.Hierarchy;
using TextCaptchaGenerator.DrawingObjects.Base;

namespace TextCaptchaGenerator.RND{
    public abstract class RNDLayer {
        protected Layer Layer {get;set;}

        public RNDLayer(SKImageInfo info){
            Layer = new Layer(info);
        }
    }
}