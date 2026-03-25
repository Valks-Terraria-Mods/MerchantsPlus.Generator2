namespace MerchantsPlus.Generator;

public sealed class MerchantBadgeColorComponent{
    public (string BackgroundHex, string ForegroundHex) BuildBadgeColors(string seed)
    {
        var hash = StringComparer.OrdinalIgnoreCase.GetHashCode(seed);
        var hue = Math.Abs(hash % 360);
        var (r, g, b) = HslToRgb(hue / 360d, 0.58, 0.38);
        var luminance = (0.299 * r) + (0.587 * g) + (0.114 * b);
        var foreground = luminance > 150 ? "#162029" : "#F4F8FD";
        return ($"#{r:X2}{g:X2}{b:X2}", foreground);
    }

    private static (byte R, byte G, byte B) HslToRgb(double h, double s, double l)
    {
        if (s <= 0)
        {
            var gray = (byte)Math.Round(l * 255d);
            return (gray, gray, gray);
        }

        var q = l < 0.5 ? l * (1 + s) : l + s - (l * s);
        var p = 2 * l - q;
        return (
            ToRgbByte(HueToChannel(p, q, h + (1d / 3d))),
            ToRgbByte(HueToChannel(p, q, h)),
            ToRgbByte(HueToChannel(p, q, h - (1d / 3d))));
    }

    private static double HueToChannel(double p, double q, double t)
    {
        if (t < 0)
            t += 1;
        if (t > 1)
            t -= 1;
        if (t < (1d / 6d))
            return p + ((q - p) * 6 * t);
        if (t < 0.5)
            return q;
        if (t < (2d / 3d))
            return p + ((q - p) * ((2d / 3d) - t) * 6);

        return p;
    }

    private static byte ToRgbByte(double channel)
    {
        return (byte)Math.Clamp((int)Math.Round(channel * 255d), 0, 255);
    }
}
