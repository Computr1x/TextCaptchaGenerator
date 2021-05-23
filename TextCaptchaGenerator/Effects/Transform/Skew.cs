using SkiaSharp;
using System;
using TextCaptchaGenerator.BaseObjects;

namespace TextCaptchaGenerator.Effects.Transform
{
    public class Skew : IEffect
    {
        public float RigthTopAngle { get; set; }
        public float LeftTopAngle { get; set; }

        public float RigthBottomAngle { get; set; }
        public float LeftBottomAngle { get; set; }
        public bool Antialiasing { get; set; } = false;

        public Skew(float leftTopAngle, float leftBottomAngle, float rigthTopAngle, float rigthBottomAngle)
        {
            RigthTopAngle = rigthTopAngle % 90 * MathF.PI / 180f;
            LeftTopAngle = leftTopAngle % 90 * MathF.PI / 180f;
            RigthBottomAngle = rigthBottomAngle % 90 * MathF.PI / 180f;
            LeftBottomAngle = leftBottomAngle % 90 * MathF.PI / 180f;
        }

        public void Draw(SKBitmap bitmap)
        {
            int width = bitmap.Width;
            int height = bitmap.Height;

            IntPtr pixelsAddr = bitmap.GetPixels();
            uint[,] buffer = new uint[height, width];

            bool topLeft = Math.Abs(LeftTopAngle) > Math.Abs(LeftBottomAngle), topRight = Math.Abs(RigthTopAngle) > Math.Abs(RigthBottomAngle);

            if (topLeft)
                LeftTopAngle -= LeftBottomAngle;
            else
                LeftBottomAngle -= LeftTopAngle;
            if (topRight)
                RigthTopAngle -= RigthBottomAngle;
            else
                RigthBottomAngle -= RigthTopAngle;


            unsafe
            {
                uint* pSrc = (uint*)pixelsAddr.ToPointer();

                float leftTan = MathF.Tan(topLeft == true ? LeftTopAngle : LeftBottomAngle),
                    rightTan = MathF.Tan(topRight == true ? RigthTopAngle : RigthBottomAngle),
                    step = 0, offsetX = 0, offsetY = 0, currentY = 0, currentY2 = 0;
                int xStart = 0, xEnd = 0;


                if (Antialiasing)
                {
                    float fractionX = 0, fractionY = 0, oneMinusX = 0, oneMinusY = 0;
                    int ceilX = 0, ceilY = 0, floorX = 0, floorY = 0;

                    for (int y = 0; y < height; y++)
                    {
                        currentY = topLeft ? y : height - y - 1;
                        xStart = (int)(leftTan * currentY);
                        xEnd = width + (int)(rightTan * currentY);

                        step = width / (float)(xEnd - xStart);
                        offsetY = y;
                        offsetX = 0;

                        for (int x = xStart; x < xEnd; x++)
                        {
                            Utils.SetAntialisedColor(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                    ref buffer, ref x, ref y,
                                    ref floorX, ref floorY, ref ceilX, ref ceilY, ref fractionX,
                                    ref fractionY, ref oneMinusX, ref oneMinusY);
                            offsetX += step;
                        }
                    }
                }
                else
                {

                    for (int y = 0; y < height; y++)
                    {
                        currentY = topLeft ? y : height - y - 1;
                        currentY2 = topRight ? y : height - y - 1;
                        
                        xStart = (int)(leftTan * currentY);
                        xEnd = width + (int)(rightTan * currentY2);

                        step = width / (float)(xEnd - xStart);
                        offsetY =  y;
                        offsetX = 0;

                        //if (xStart < 0)
                        //    xStart = 0;
                        //if (xEnd > width)
                        //    xEnd = width;

                        for (int x = xStart; x < xEnd; x++)
                        {
                            Utils.SetColorCheckSrcDst(pSrc, ref width, ref height, ref offsetX, ref offsetY,
                                        ref buffer, ref x, ref y);
                            offsetX += step;
                        }
                    }
                }


                fixed (uint* newPtr = buffer)
                {
                    bitmap.SetPixels((IntPtr)newPtr);
                }
            }
        }
    }
}
