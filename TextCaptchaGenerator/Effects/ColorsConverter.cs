using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects
{
    // Color class from my Igor' <3
    public static class ColorsConverter
    {
		/// <summary>Converts HSB color model values to RGB values</summary>
		/// <exception cref="OverflowException">Thrown when any parameter is out of range [0; 1]</exception>
		public static void HsbToRgb(in float h, in float s, in float v, out float r, out float g, out float b)
		{
			//if (h is < 0 or > 1 || s is < 0 or > 1 || v is < 0 or > 1)
			//	throw new OverflowException();
			float hCopy = h / (1f / 6f);

			var integer = (int)hCopy;
			var fractional = hCopy - integer;

			float
				p = v * (1 - s),
				q = v * (1 - (fractional * s)),
				t = v * (1 - ((1 - fractional) * s));

			// Hue sector to rgb
			switch (integer)
			{
				case 1:
					r = q;
					g = v;
					b = p;

					break;
				case 2:
					r = p;
					g = v;
					b = t;

					break;
				case 3:
					r = p;
					g = q;
					b = v;

					break;
				case 4:
					r = t;
					g = p;
					b = v;

					break;
				case 5:
					r = v;
					g = p;
					b = q;

					break;
				default:
					r = v;
					g = t;
					b = p;

					break;
			}
		}

		/// <summary>Converts RGB color model values to HSB values</summary>
		/// <exception cref="OverflowException">Thrown when any parameter is out of range [0; 1]</exception>
		public static void RgbToHsb(in float r, in float g, in float b, out float h, out float s, out float v)
		{
			//if (r is < 0 or > 1 || g is < 0 or > 1 || b is < 0 or > 1)
			//	throw new OverflowException();

			h = 0;
			v = r > g ? (r > b ? r : b) : (g > b ? g : b);

			var delta = v - (r < g ? (r < b ? r : b) : (g < b ? g : b));

			s = v == 0 ? 0 : delta / v;

			if (s != 0)
			{
				// Determining hue sector
				if (r == v)
				{
					h = (g - b) / delta;
				}
				else if (g == v)
				{
					h = 2 + (b - r) / delta;
				}
				else if (b == v)
				{
					h = 4 + (r - g) / delta;
				}

				// Sector to hue
				h *= (1f / 6f);

				// For cases like R = MAX & B > G
				if (h < 0)
					h += 1f;
			}
		}

		public static RGBColor HsbToColor(in float h, in float s, in float v)
		{
			//Debug.WriteLine("{0}, {1}, {2}", h, s, v);

			HsbToRgb(h, s, v, out var r, out var g, out var b);

			return RgbToColor(r, g, b);
		}

		public static bool IsBright(in RGBColor source)
		{
			//return (source.R + source.G + source.B) / 3 > 0x7E;
			return source.R * 0.2126 + source.G * 0.7152 + source.B * 0.0722 > 127.5;
		}

		private static RGBColor _black = new RGBColor(255, 255, 255, 255), _white = new RGBColor(255, 0, 0, 0);
		public static RGBColor IsBrightToColor(in RGBColor source)
		{
			return IsBright(source) ? _black : _white;
		}

		public static RGBColor CriticalPercentageToColor(in float value, in float min, in float max, in float s, in float b)
		{
			return HsbToColor(min + value * (max - min), s, b);
		}

		//public static ARGBColor AnotherAlpha(ARGBColor source, byte a)
		//{
		//	return ARGBColor.FromArgb(a, source.R, source.G, source.B);
		//}

		//public static ARGBColor Multiply(ARGBColor source, float m)
		//{
		//	return ARGBColor.FromArgb(
		//		byte.MaxValue,
		//		(byte)Math.Min(source.R * m, 0xFF),
		//		(byte)Math.Min(source.G * m, 0xFF),
		//		(byte)Math.Min(source.B * m, 0xFF)
		//	);
		//}

		public static RGBColor RgbToColor(in float a, in float r, in float g, in float b)
		{
			return new RGBColor((byte)(a * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}

		public static RGBColor RgbToColor(in float r, in float g, in float b)
		{
			return new RGBColor((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}

		public static uint RgbToUint(in float a, in float r, in float g, in float b)
		{
			return (uint) (((byte)(a * 255)) << 24 | ((byte)(r * 255)) << 16 | ((byte)(g * 255)) << 8 | ((byte)(b * 255)));
		}

		public static uint RgbToUint(in float r, in float g, in float b)
		{
			return (uint)(255 << 24 | ((byte)(r * 255)) << 16 | ((byte)(g * 255)) << 8 | ((byte)(b * 255)));
		}


		public static void UintToRgb(in uint rgb, out byte r, out byte g, out byte b)
		{
			r = (byte)(rgb >> 16 & 0xFF);
			g = (byte)(rgb >> 8 & 0xFF);
			b = (byte)(rgb & 0xFF);
		}

		public static void UintToArgb(in uint argb, out byte a, out byte r, out byte g, out byte b)
		{
			a = (byte)(argb >> 24);
			r = (byte)(argb >> 16 & 0xFF);
			g = (byte)(argb >> 8 & 0xFF);
			b = (byte)(argb & 0xFF);
		}

		public static RGBColor UintToBColor(in byte a, in uint rgb)
		{
			UintToRgb(rgb, out var r, out var g, out var b);

			return new RGBColor(a, r, g, b);
		}

		public static RGBColor UintToBColor(in uint rgb)
		{
			return UintToBColor(byte.MaxValue, rgb);
		}

		public static HSBColor UintToFColor(in byte a, in uint rgb)
		{
			UintToRgb(rgb, out var r, out var g, out var b);

			return new HSBColor(a, r, g, b);
		}

		public static HSBColor UintToFColor(in uint rgb)
		{
			return UintToFColor(byte.MaxValue, rgb);
		}


		public static uint ArgbToUint(in byte a, in byte r, in byte g, in byte b)
		{
			return (uint)(a << 24 | r << 16 | g << 8 | b);
		}

		public static uint RgbToUint(in byte r, in byte g, in byte b)
		{
			return (uint)(r << 16 | g << 8 | b);
		}

		public static uint RgbFToUint(in float r, in float g, in float b)
		{
			return RgbToUint((byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}

		public static uint ArgbFToUint(in float a, in float r, in float g, in float b)
		{
			return ArgbToUint((byte)(a * 255), (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
		}

		public static uint ColorToUint(in RGBColor color)
		{
			return RgbToUint(color.R, color.G, color.B);
		}

		public static void GetRandomHsb(in Random rand, in float s, in float v, out float r, out float g, out float b)
		{
			HsbToRgb((float)rand.NextDouble(), s, v, out r, out g, out b);
		}

		public static RGBColor GetRandomHsb(in Random rand,float a, in float s, in float v)
		{
			GetRandomHsb(rand, s, v, out var r, out var g, out var b);

			return RgbToColor(a, r, g, b);
		}

		public static RGBColor GetRandomHsb(in Random rand, in float s, in float v)
		{
			return GetRandomHsb(rand, 1, s, v);
		}

		public static uint GetRandomUintHsb(in Random rand, in float s, in float v)
		{
			GetRandomHsb(rand, s, v, out var r, out var g, out var b);

			return RgbToUint(r, g, b);
		}
	}
}
