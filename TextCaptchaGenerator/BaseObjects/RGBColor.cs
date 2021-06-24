using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TextCaptchaGenerator.BaseObjects
{
    public struct RGBColor
    {
        public byte A, R, G, B;

        public RGBColor(byte a, byte r, byte g, byte b)
        {
            A = a;
            R = r;
            G = g;
            B = b;
        }

        public RGBColor(byte r, byte g, byte b)
        {
            A = 255;
            R = r;
            G = g;
            B = b;
        }
    }
}
