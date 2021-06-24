using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextCaptchaGenerator.Effects;

namespace TextCaptchaGenerator.BaseObjects
{
    public struct HSBColor
    {
        public float A, H, S, B;

        public HSBColor(float a, float h, float s, float b)
        {
            A = a;
            H = h;
            S = s;
            B = b;
        }

        public HSBColor(byte a, byte h, byte s, byte b)
        {
            A = a / 256f;
            H = h / 256f;
            S = s / 256f;
            B = b / 256f;
        }

        //public uint ToUint()
        //{
        //    return Utils.MakePixel(A, H, S, B);
        //}
    }
}
