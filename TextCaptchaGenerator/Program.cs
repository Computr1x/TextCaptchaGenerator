using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using TextCaptchaGenerator.DrawingObjects.Base;
using TextCaptchaGenerator.Effects.Distort;
using TextCaptchaGenerator.Effects.Noise;
using TextCaptchaGenerator.Effects.Transform;
using TextCaptchaGenerator.Hierarchy;

namespace TextCaptchaGenerator
{
    class Program
    {
		private static string dataPath = CreateDataPath();

		private static string CreateDataPath()
        {
			string path = Path.GetFullPath("..\\..\\..\\data");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return path;
        }

        static void Main(string[] args)
        {
			Test($"test1.png");
		}

		private static void Test(string name)
		{
			Image image = new Image(512, 256);

			// layer1 
			Layer layer1 = new Layer(image.info, SKColors.White);
			DEllipse dEllipse = new DEllipse(new SKPoint(15, 15), 40, new SKPaint() { Color = new SKColor(0xff4285F4), IsAntialias = true });
			DText dText = new DText(new SKPoint(128, 128), "Igor kek", new SKFont(),
				new SKPaint
				{
					Color = new SKColor(0xff4285F4),
					IsAntialias = true,
					Style = SKPaintStyle.Fill,
					StrokeWidth = 4
				});

			layer1.Drawables.Add(dEllipse);
			layer1.Drawables.Add(dText);
			image.Layers.Add(layer1);

			// layer2
			Layer layer2 = new Layer(image.info, SKColors.Transparent);
			// objects
			DRectangle dRect = new DRectangle(new SKRect(200, 225, 55, 55), new SKPaint() { Color = new SKColor(0xd0428522), IsAntialias = true });
			DLine dLine = new DLine(new SKPoint(25, 65), new SKPoint(65, 25), new SKPaint() { Color = SKColors.IndianRed, IsAntialias = true, StrokeWidth = 12 });
			DLine dLine2 = new DLine(new SKPoint(5, 250), new SKPoint(512, 250), new SKPaint() { Color = SKColors.Purple, IsAntialias = true, StrokeWidth = 5 });
			DLine dLine3 = new DLine(new SKPoint(1, 1), new SKPoint(1, 512), new SKPaint() { Color = SKColors.Brown, IsAntialias = true, StrokeWidth = 2 });
			DLine dLine4 = new DLine(new SKPoint(511, 1), new SKPoint(511, 255), new SKPaint() { Color = SKColors.Blue, IsAntialias = true, StrokeWidth = 3 });
			DPolygon dPolygon = new DPolygon(new SKPoint[] { new SKPoint (350, 5), new SKPoint(350, 250), new SKPoint(450, 175)}, 
				new SKPaint() { Color = SKColors.Yellow, IsAntialias = true, StrokeWidth = 3, Style = SKPaintStyle.StrokeAndFill });

			layer2.Drawables.Add(dRect);
			layer2.Drawables.Add(dLine);
			layer2.Drawables.Add(dLine2);
			layer2.Drawables.Add(dLine3);
			layer2.Drawables.Add(dLine4);
			layer2.Drawables.Add(dPolygon);
			image.Layers.Add(layer2);

			// distort
			//Swirl effect = new Swirl(150, 2, 30, 30) { Antialiasing = true };
			//Wave effect = new Wave(35, 6, Wave.eWaveType.Sine) { Antialiasing = true };
			//Bulge effect = new Bulge(64, 64, 100, -1);
			//Ripple effect = new Ripple(50, 50);
			//SlitScan effect = new SlitScan();

			// transform
			//Scale effect = new Scale(1f, 0.9f);
			//Shift effect = new Shift(0, 100);
			//Flip effect = new Flip(Flip.eFlipType.Both);
			//Rotate effect = new Rotate(45);
			//Skew effect = new Skew(30, 0, 0, 30);


			// noise
			//GaussNoise effect = new GaussNoise(100) { Monochrome = true };
			PerlinNoise effect = new PerlinNoise() { Octaves = 10, Step = 5, Persistence = 0.5f };
            layer1.Effects.Add(effect);

            //PolarCoordinates effect = new PolarCoordinates() { Antialiasing = true };
            //layer2.Effects.Add(effect);
            //PolarCoordinates effect2 = new PolarCoordinates(PolarCoordinates.ePolarType.PolarToRectangular) { Antialiasing = true };
            //layer2.Effects.Add(effect2);


            // image to png
            using (var res = image.DrawAll())
			{
				using (var data = res.Encode(SKEncodedImageFormat.Png, 100))
				using (var stream = File.OpenWrite(Path.Combine(dataPath, name)))
				{
					data.SaveTo(stream);
				}
			}
		}

		private static void TestBlending()
        {
			foreach (SKBlendMode blendMode in Enum.GetValues(typeof(SKBlendMode)))
			{
				Image image = new Image(256, 256);

				// layer1 
				Layer layer1 = new Layer(image.info, blendMode, SKColors.White);
				DEllipse dEllipse = new DEllipse(new SKPoint(15, 15), 40, new SKPaint() { Color = new SKColor(0xff4285F4), IsAntialias = true });
				DText dText = new DText(new SKPoint(128, 128), "Igor kek", new SKFont(),
					new SKPaint
					{
						Color = new SKColor(0xff4285F4),
						IsAntialias = true,
						Style = SKPaintStyle.Fill,
						StrokeWidth = 4
					});

				layer1.Drawables.Add(dEllipse);
				layer1.Drawables.Add(dText);
				image.Layers.Add(layer1);

				// layer2
				Layer layer2 = new Layer(image.info, blendMode, SKColors.Transparent);
				DRectangle dRect = new DRectangle(new SKRect(200, 200, 55, 55), new SKPaint() { Color = new SKColor(0xd0428522), IsAntialias = true });
				DLine dLine = new DLine(new SKPoint(25, 65), new SKPoint(65, 25), new SKPaint() { Color = SKColors.IndianRed, IsAntialias = true, StrokeWidth = 12 });

				layer2.Drawables.Add(dRect);
				layer2.Drawables.Add(dLine);
				image.Layers.Add(layer2);


				// image to png
				using (var res = image.DrawAll())
				{
					using (var data = res.Encode(SKEncodedImageFormat.Png, 100))
					using (var stream = File.OpenWrite(Path.Combine(dataPath, blendMode.ToString() + ".png")))
					{
						data.SaveTo(stream);
					}
				}
			}
		}
    }
}
