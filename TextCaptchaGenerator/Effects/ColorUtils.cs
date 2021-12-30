using System.Runtime.CompilerServices;
using TextCaptchaGenerator.BaseObjects;

public static class ColorUtils {

    #region Get channel of color
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetAlphaChannel(in uint sourceColor){
        return (sourceColor & 0xff000000) >> 24;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetRedChannel(in uint sourceColor){
        return (sourceColor & 0x00ff0000) >> 16;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetGreenChannel(in uint sourceColor){
        return (sourceColor & 0x0000ff00) >> 8;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint GetBlueChannel(in uint sourceColor){
        return (sourceColor & 0x000000ff);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetAlphaFChannel(in uint sourceColor){
        return GetAlphaChannel(sourceColor) / 255f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetRedFChannel(in uint sourceColor){
        return GetRedChannel(sourceColor) / 255f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetGreenFChannel(in uint sourceColor){
        return GetGreenChannel(sourceColor) / 255f;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float GetBlueFChannel(in uint sourceColor){
        return GetBlueChannel(sourceColor) / 255f;
    }
    #endregion

    # region fromColorConverter
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
    #endregion
}