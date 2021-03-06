using SkiaSharp;
using System;
using System.IO;
using System.Collections.Generic;
using TextCaptchaGenerator.DrawingObjects.Base;
using TextCaptchaGenerator.DrawingObjects;
using TextCaptchaGenerator.Hierarchy;
using TextCaptchaGenerator.RND;
using TextCaptchaGenerator.RND.Range;
using TextCaptchaGenerator.RND.Layers;
using TextCaptchaGenerator.Effects.Glitch;
using TextCaptchaGenerator.Effects.Distort;
using TextCaptchaGenerator.Effects.Color;
using TextCaptchaGenerator.Effects.Noise;
using TextCaptchaGenerator.Effects.Transform;

namespace TextCaptchaGenerator
{
    class Program
    {
        private static readonly string DataPath = CreateDataPath();

        private static string CreateDataPath()
        {
            string path = Path.GetFullPath("./TextCaptchaGenerator/.data");

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }

        static void Main(string[] args)
        {
            Test($"test1.png");
            Console.WriteLine("Done");
        }

        private static void Test(string name)
        {
            Image image = new Image(512, 256);

            RNDManager mgr = new RNDManager(0);
            RNDObjectManager objMgr = new((RNDRectangle)image.info, mgr);
            RNDEffectManager effMgr = new((RNDRectangle)image.info, mgr);

            // layer1 
            Layer layer1 = new Layer(image.info, SKColors.White);
            layer1.Drawables.AddRange(objMgr.GetRandomDrawable(5));
            layer1.Effects.AddRange(effMgr.GetRandomEffects(3));
            image.Layers.Add(layer1);


            Layer layer2 = new Layer(image.info);
            layer2.Drawables.Add(
                objMgr.GetRandomText()
            );
            layer2.Effects.AddRange(effMgr.GetRandomEffects(2));
            image.Layers.Add(layer2);
            
            Layer layer3 = new Layer(image.info, SKColors.White, 40);
            layer3.Drawables.AddRange(objMgr.GetRandomDrawable(8));
            layer3.Effects.AddRange(effMgr.GetRandomEffects(3));
            // var scaleEff = new Scale(0.8f, 1.8f);
            // layer3.Effects.Add(scaleEff);
            image.Layers.Add(layer3);


            // image to png
            using var res = image.DrawAll();
            using var data = res.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = File.OpenWrite(Path.Combine(DataPath, name));
            data.SaveTo(stream);
        }



/*
            // layer2
            Layer layer2 = new Layer(image.info, SKColors.Transparent, 200);
            // objects
            DRectangle dRect = new DRectangle(
                new SKRect(200, 225, 55, 55),
                new SKPaint()
                {
                    // Color = new SKColor(0xd0428522), 
                    IsAntialias = true,
                    Shader = SKShader.CreateLinearGradient(
                    new SKPoint(200, 255),
                    new SKPoint(55, 55),
                    new[] { SKColors.AliceBlue, SKColors.Green },
                    SKShaderTileMode.Clamp)
                });
            DLine dLine = new DLine(new SKPoint(25, 65), new SKPoint(65, 25), new SKPaint() { Color = SKColors.IndianRed, IsAntialias = true, StrokeWidth = 12 });
            DLine dLine2 = new DLine(new SKPoint(5, 250), new SKPoint(512, 250), new SKPaint() { Color = SKColors.Purple, IsAntialias = true, StrokeWidth = 5 });
            DLine dLine3 = new DLine(new SKPoint(1, 1), new SKPoint(1, 512), new SKPaint() { Color = SKColors.Brown, IsAntialias = true, StrokeWidth = 2 });
            DLine dLine4 = new DLine(new SKPoint(511, 1), new SKPoint(511, 255), new SKPaint() { Color = SKColors.Blue, IsAntialias = true, StrokeWidth = 3 });
            DPolygon dPolygon =
                new DPolygon(new SKPoint[] {new SKPoint(350, 5), new SKPoint(350, 250), new SKPoint(450, 175)},
                    new SKPaint()
                        {
                            Color = SKColors.Yellow,
                            IsAntialias = true,
                            StrokeWidth = 3,
                            Style = SKPaintStyle.StrokeAndFill
                        });
            DImage dImage = new DImage(200, 150, Path.Combine(DataPath, "img.png"));

            DText dText = new DText(new SKPoint(200, 128), 
                "Igor kek",
                new SKPaint()
                {
                    Color = new SKColor(0xff4285F4),
                    IsAntialias = true,
                    Style = SKPaintStyle.Fill,
                    StrokeWidth = 4,
                    TextSize = 40
                });
            layer2.Drawables.Add(dText);

            

            
            Layer layer3 = new RNDObjectsLayer(image.info, mgr, null, 100, 5);
            image.Layers.Add(layer3);
            layer3.Effects.AddRange(effMgr.GetRandomEffects(3));

            Layer layer4 = new Layer(image.info);
            layer4.Drawables.Add(
                objMgr.GetRandomText()
            );
            // RGBShift effect = new RGBShift(3);
            // layer4.Effects.Add(effect);
            layer4.Effects.AddRange(effMgr.GetRandomEffects(3));
            image.Layers.Add(layer4);

            // layer2.Drawables.AddRange(new List<BaseDrawable>(){
            //     dRect, dLine, dLine2, dLine3, dLine4, dPolygon, dImage
            // });
            // image.Layers.Add(layer2);

            // Layer layer3 = new Layer(image.info, SKColors.Transparent);
            // DCurveLine dCurveLine = new DCurveLine(
            //     new SKPoint[] { new(20,20), new(40,40), new(180, 100), new(200, 150), new(250,250)},
            //     new SKPaint() { 
            //         Color = SKColors.MistyRose,
            //         StrokeWidth = 6,
            //         IsAntialias = true,
            //         Style = SKPaintStyle.Stroke
            //         }
            //     );
            // DSVGPathPattern dSVGPathPattern = new DSVGPathPattern(
            //     new SKRect(0,0,300,250),
            //     DSVGPathPattern.triangle,
            //     new SKPaint(){
            //         Color = SKColors.Black,
            //         IsAntialias = true
            //     }
            // );
            // layer3.Drawables.AddRange(new List<BaseDrawable>(){
            //     dCurveLine, dSVGPathPattern
            // });
            // image.Layers.Add(layer3);


            


            // distort
            //Swirl effect = new Swirl(150, 2, 30, 30) { Antialiasing = true };
            //Wave effect = new Wave(35, 6, Wave.eWaveType.Sine) { Antialiasing = true };
            // Bulge effect = new Bulge(64, 64, 100, -1);
            // Ripple effect = new Ripple(30, 30);
            // SlitScan effect = new SlitScan();

            // transform
            //Scale effect = new Scale(1f, 0.9f);
            //Shift effect = new Shift(0, 100);
            //Flip effect = new Flip(Flip.eFlipType.Both);
            //Rotate effect = new Rotate(45);
            //Skew effect = new Skew(30, 0, 0, 30);


            // noise
            // GaussNoise effect = new GaussNoise(255) { Monochrome = true };
            //PerlinNoise effect = new PerlinNoise() { Octaves = 10, Step = 5, Persistence = 0.5f, Monochrome = false };


            // glitch
            // RGBShift effect = new RGBShift(3);
            // Pixelate effect = new Pixelate(10, 4);
            // Slices effect2 = new Slices() { Count = 10, SliceHeight = 10 };
            // Crystallize effect = new Crystallize() { CrystalsCount = 512 };
            // layer2.Effects.Add(effect);

            // EdgeDetection effect = new EdgeDetection(
            // 	new float[,]{
            // 		{1f/9f, 1f/9f, 1f/9f},
            // 		{1f/9f, 1f/9f, 1f/9f},
            // 		{1f/9f, 1f/9f, 1f/9f}
            // 	}
            // );
            // layer2.Effects.Add(effect);

            // TODO Fix dithering
            // FSDithering effect = new FSDithering() { GrayScale = true };
            // layer2.Effects.Add(effect);



            // color
            //GrayScale effect = new GrayScale();
            //HSBCorrection effect = new HSBCorrection(0, 5, 5);


            //PolarCoordinates effect = new PolarCoordinates() { Antialiasing = true };
            //layer2.Effects.Add(effect);
            //PolarCoordinates effect2 = new PolarCoordinates(PolarCoordinates.ePolarType.PolarToRectangular) { Antialiasing = true };
            //layer2.Effects.Add(effect2);


        private static void TestBlending()
        {
            foreach (SKBlendMode blendMode in Enum.GetValues(typeof(SKBlendMode)))
            {
                Image image = new Image(512, 256);

                // layer1 
                Layer layer1 = new Layer(image.info, blendMode, SKColors.White);
                DEllipse dEllipse = new DEllipse(new SKPoint(15, 15), 40, new SKPaint() { Color = new SKColor(0xff4285F4), IsAntialias = true });

                DText dText = new DText(new SKPoint(128, 128), "Igor kek", 
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
                    using (var stream = File.OpenWrite(Path.Combine(DataPath, blendMode.ToString() + ".png")))
                    {
                        data.SaveTo(stream);
                    }
                }
            }
        }
*/
    }
}
