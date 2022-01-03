using System;
using System.Text;
using System.Collections.Generic;
using SkiaSharp;
using TextCaptchaGenerator.Effects;
using TextCaptchaGenerator.RND;
using TextCaptchaGenerator.RND.Range;
using TextCaptchaGenerator.Hierarchy;
using TextCaptchaGenerator.DrawingObjects.Base;
using TextCaptchaGenerator.BaseObjects.Enums;

namespace TextCaptchaGenerator.RND.Layers {
    public class RNDObjectsLayer : Layer {
        public RNDObjectsLayer(RNDManager manager, SKImageInfo info, 
            byte opacity = 255, int objectsCount = 3)
            : base(info, opacity: opacity){
            
            RNDRectangle rect = (RNDRectangle)info.Rect;
            RNDObjectManager objManager = new(rect, manager);

            Drawables.AddRange(objManager.GetRandomDrawable(objectsCount));
        }
    }
}