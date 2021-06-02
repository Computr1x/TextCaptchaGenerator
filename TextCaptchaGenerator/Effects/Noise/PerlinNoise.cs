using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Noise
{
    public class PerlinNoise : IEffect
    {
        private class Vector2D
        {
            public float X { get; set; }
            public float Y { get; set; }

            public Vector2D()
            {
            }

            public Vector2D(float x, float y)
            {
                X = x;
                Y = y;
            }
        }

        byte[] permutationTable;

        public int Step { get; set; } = 1;

        public bool Monochrome { get; set; } = false;

        public int Octaves { get; set; } = 5;

        public float Persistence { get; set; } = 0.5f;

        public PerlinNoise(int seed = 0)
        {
            var rand = new Random(seed);
            permutationTable = new byte[1024];
            rand.NextBytes(permutationTable);
        }

        public void Draw(SKBitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            //float mean = Amount, stdDev = (1f - mean) / 6f;

            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                Random rand = new Random();
                byte noise = 0;
                uint color = 0;

                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        uint colorSrc = *(pSrc + (y * width + x));
                        if (colorSrc >> 24 == 0)
                            continue;

                        noise = (byte)(MathF.Round((Noise(x / (float)width, y / (float)height, Octaves, Persistence) + 1f / 2f) * 255 / Step) * Step);
                        //buffer[y, x] = ConvertTotalToRgb((int) (MathF.Round(noise * 255 / Step) * Step));

                        color = Monochrome ? Utils.MakePixel(noise, noise, noise, 255) : ConvertTotalToRgb(noise);

                        buffer[y, x] = Utils.MultiplyFloatToPixel(color, colorSrc);
                    }
                }

                fixed (uint* newPtr = buffer)
                {
                    bitmap.SetPixels((IntPtr)newPtr);
                }
            }
        }

        public static uint ConvertTotalToRgb(byte value)
        {
            return HSVtoRGB(value / 255f, 1.0f, 1.0f, 1.0f);
        }


        public static uint HSVtoRGB(float hue, float saturation, float value, float alpha)
        {
            while (hue > 1f) { hue -= 1f; }
            while (hue < 0f) { hue += 1f; }
            while (saturation > 1f) { saturation -= 1f; }
            while (saturation < 0f) { saturation += 1f; }
            while (value > 1f) { value -= 1f; }
            while (value < 0f) { value += 1f; }
            if (hue > 0.999f) { hue = 0.999f; }
            if (hue < 0.001f) { hue = 0.001f; }
            if (saturation > 0.999f) { saturation = 0.999f; }
            if (saturation < 0.001f) { return Utils.MakePixel((byte)(value * 255), (byte)(value * 255), (byte)(value * 255), (byte)(alpha * 255)); }
            if (value > 0.999f) { value = 0.999f; }
            if (value < 0.001f) { value = 0.001f; }

            float h6 = hue * 6f;
            if (h6 == 6f) { h6 = 0f; }
            int ihue = (int)(h6);
            float p = value * (1f - saturation);
            float q = value * (1f - (saturation * (h6 - ihue)));
            float t = value * (1f - (saturation * (1f - (h6 - ihue))));

            switch (ihue)
            {
                case 0:
                    return Utils.MakePixel((byte)(value * 255), (byte)(t * 255), (byte)(p * 255), (byte)(alpha * 255));
                case 1:
                    return Utils.MakePixel((byte)(q * 255), (byte)(value * 255), (byte)(p * 255), (byte)(alpha * 255));
                case 2:
                    return Utils.MakePixel((byte)(p * 255), (byte)(value * 255), (byte)(t * 255), (byte)(alpha * 255));
                case 3:
                    return Utils.MakePixel((byte)(p * 255), (byte)(q * 255), (byte)(value * 255), (byte)(alpha * 255));
                case 4:
                    return Utils.MakePixel((byte)(t * 255), (byte)(p * 255), (byte)(value * 255), (byte)(alpha * 255));
                default:
                    return Utils.MakePixel((byte)(value * 255), (byte)(p * 255), (byte)(q * 255), (byte)(alpha * 255));
            }
        }


        private Vector2D GetPseudoRandomGradientVector(int x, int y)
        {
            int v = permutationTable[(int)(((x * 1836311903) ^ (y * 2971215073) + 4807526976) & 1023)] & 3;
            //v = permutationTable[v] & 3;

            switch (v)
            {
                case 0: return new Vector2D(1, 0);
                case 1: return new Vector2D(-1, 0);
                case 2: return new Vector2D(0, 1);
                default: return new Vector2D(0, -1);
            }
        }

        static float QunticCurve(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }

        static float Dot(Vector2D a, Vector2D b)
        {
            return a.X * b.X + a.Y * b.Y;
        }

        private float Noise(float fx, float fy)
        {
            int left = (int)MathF.Floor(fx);
            int top = (int)MathF.Floor(fy);
            float pointInQuadX = fx - left;
            float pointInQuadY = fy - top;

            Vector2D topLeftGradient = GetPseudoRandomGradientVector(left, top);
            Vector2D topRightGradient = GetPseudoRandomGradientVector(left + 1, top);
            Vector2D bottomLeftGradient = GetPseudoRandomGradientVector(left, top + 1);
            Vector2D bottomRightGradient = GetPseudoRandomGradientVector(left + 1, top + 1);

            Vector2D distanceToTopLeft = new Vector2D(pointInQuadX, pointInQuadY);
            Vector2D distanceToTopRight = new Vector2D(pointInQuadX - 1, pointInQuadY);
            Vector2D distanceToBottomLeft = new Vector2D(pointInQuadX, pointInQuadY - 1);
            Vector2D distanceToBottomRight = new Vector2D(pointInQuadX - 1, pointInQuadY - 1);

            float tx1 = Dot(distanceToTopLeft, topLeftGradient);
            float tx2 = Dot(distanceToTopRight, topRightGradient);
            float bx1 = Dot(distanceToBottomLeft, bottomLeftGradient);
            float bx2 = Dot(distanceToBottomRight, bottomRightGradient);

            pointInQuadX = QunticCurve(pointInQuadX);
            pointInQuadY = QunticCurve(pointInQuadY);

            float tx = Lerp(tx1, tx2, pointInQuadX);
            float bx = Lerp(bx1, bx2, pointInQuadX);
            float tb = Lerp(tx, bx, pointInQuadY);

            return tb;
        }

        private float Noise(float fx, float fy, int octaves, float persistence = 0.5f)
        {
            float amplitude = 1;
            float max = 0;
            float result = 0;

            while (octaves-- > 0)
            {
                max += amplitude;
                result += Noise(fx, fy) * amplitude;
                amplitude *= persistence;
                fx *= 2;
                fy *= 2;
            }

            return result / max;
        }
    }
}
