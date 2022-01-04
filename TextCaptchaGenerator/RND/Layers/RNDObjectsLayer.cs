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
        public RNDObjectsLayer(SKImageInfo info, RNDManager manager, RNDObjectManager objectManager = null,
          byte opacity = 255, int objectsCount = 3)
            : base(info, opacity: opacity){
            
            RNDRectangle rect = (RNDRectangle)info.Rect;
            objectManager ??= 
                new(rect, manager, 
                    new(new List<eDrawableType>(){
                        eDrawableType.Ellipse, 
                        eDrawableType.Line, 
                        eDrawableType.Polygon, 
                        eDrawableType.Rectangle, 
                        //eDrawableType.Text
                        }));

            Drawables.AddRange(objectManager.GetRandomDrawable(objectsCount));
        }
    }
}