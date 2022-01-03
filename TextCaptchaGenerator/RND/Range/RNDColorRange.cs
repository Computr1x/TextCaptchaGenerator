using TextCaptchaGenerator.BaseObjects;
using TextCaptchaGenerator.Effects;


namespace TextCaptchaGenerator.RND.Range{
    public class RNDColorRange {
        public uint[] Colors {get; private set;}
        public byte Opacity {get; private set;}

        public RNDColorRange(uint[] colors, byte opacity = 255){
            Colors = colors;
            Opacity = opacity;
        }

        public RNDColorRange(byte opacity = 255){
            Colors = GeneratePalette(255);
            Opacity = opacity;
        }

        public RNDColorRange(int colorsCount, byte opacity = 255){
            Colors = GeneratePalette(colorsCount);
            Opacity = opacity;
        }

        private uint[] GeneratePalette(int colorsCount){
            uint[] colors = new uint[colorsCount];
            float curHue = 0, hueStep = 255f / colorsCount;

            for(int i = 0; i < colorsCount; i++)
            {
                var rgbColor = ColorsConverter.HsbFToRGBColor(in curHue, 1, 1);
                var color = ColorUtils.ColorToUint(in rgbColor);
                ColorUtils.SetAlphaChannel(ref color, Opacity);
                colors[i] = color;
                curHue += hueStep;
            }
            return colors;
        }
    }
}