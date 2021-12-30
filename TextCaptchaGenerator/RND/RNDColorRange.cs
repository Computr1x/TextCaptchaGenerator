using TextCaptchaGenerator.BaseObjects;
using TextCaptchaGenerator.Effects;


namespace TextCaptchaGenerator.RND {
    public class RNDColorRange {
        public uint[] Colors {get; private set;}

        public RNDColorRange(uint[] colors){
            Colors = colors;
        }

        public RNDColorRange(){
            Colors = GeneratePalette(255);
        }

        public RNDColorRange(int colorsCount){
            Colors = GeneratePalette(colorsCount);
        }

        private uint[] GeneratePalette(int colorsCount){
            uint[] colors = new uint[colorsCount];
            float curHue = 0, hueStep = 255f / colorsCount;

            for(int i = 0; i < colorsCount; i++)
            {
                var rgbColor = ColorsConverter.HsbFToRGBColor(in curHue, 1, 1);
                var color = ColorUtils.ColorToUint(in rgbColor);
                colors[i] = color;
                curHue += hueStep;
            }
            return colors;
        }
    }
}