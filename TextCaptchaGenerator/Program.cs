using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using TextCaptchaGenerator.DrawingObjects;
using TextCaptchaGenerator.Hierarchy;

namespace TextCaptchaGenerator
{
    class Program
    {
		private static string dataPath = CreateDataPath();

		private static string CreateDataPath()
        {
			string path = Path.GetFullPath("..\\..\\..\\.\\data");

			if (!Directory.Exists(path))
				Directory.CreateDirectory(path);

			return path;
        }

        static void Main(string[] args)
        {
			Test();

		}

		private static void Test()
		{
			Image image = new Image(256, 256);

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
			DRectangle dRect = new DRectangle(new SKRect(200, 200, 55, 55), new SKPaint() { Color = new SKColor(0xd0428522), IsAntialias = true });
			DLine dLine = new DLine(new SKPoint(25, 65), new SKPoint(65, 25), new SKPaint() { Color = SKColors.IndianRed, IsAntialias = true, StrokeWidth = 12 });

			layer2.Drawables.Add(dRect);
			layer2.Drawables.Add(dLine);
			image.Layers.Add(layer2);


			// image to png
			using (var res = image.DrawAll())
			{
				using (var data = res.Encode(SKEncodedImageFormat.Png, 100))
				using (var stream = File.OpenWrite(Path.Combine(dataPath, "test.png")))
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
