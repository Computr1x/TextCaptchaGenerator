namespace TextCaptchaGenerator.BaseObjects.Enums
{
    public enum eEffectType
    {
        // Color
        GrayScale, HSBCorrection,
        // Conv
        EdgeDetection,
        // Distort
        Bulge, PolarCoordinates, Ripple, SlitScan, Swirl, Wave,
        // Glitch
        Crystalyze, FSDithering, Pixelate, RGBShift, Slices,
        // Noise
        GaussNoise, PerlinNoise,
        // Transform
        Flip, Rotate, Scale, Shift, Skew
    }
}