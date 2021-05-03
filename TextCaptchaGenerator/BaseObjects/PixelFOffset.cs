using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.BaseObjects
{
    public struct PixelFOffset
    {
        public float X, Y;

        public PixelFOffset(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
