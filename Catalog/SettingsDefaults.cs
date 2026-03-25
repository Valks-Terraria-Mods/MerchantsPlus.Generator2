namespace MerchantsPlus.Generator;

public static class SettingsDefaults
{
    public const double TextScaleMin = 0.75;
    public const double TextScaleMax = 2.0;
    public const double TextScaleStep = 0.05;
    public const double AppTextScaleDefault = 1.0;
    public const double TreeViewTextScaleDefault = 1.0;
    public const double BaseAppFontSize = 14.0;
    public const double BaseTreeViewFontSize = 14.0;
    public const string ThemeSystem = "System";
    public const string ThemeDark = "Dark";
    public const string ThemeLight = "Light";
    public const string AppThemeDefault = ThemeDark;

    public static readonly IReadOnlyList<string> AppThemes = [ThemeSystem, ThemeDark, ThemeLight];

    public static readonly IReadOnlyList<string> AllowedMerchantNpcIds =
    [
        "Guide",
        "Merchant",
        "Nurse",
        "Demolitionist",
        "DyeTrader",
        "Angler",
        "BestiaryGirl",
        "Dryad",
        "Painter",
        "Golfer",
        "ArmsDealer",
        "DD2Bartender",
        "Stylist",
        "GoblinTinkerer",
        "WitchDoctor",
        "Clothier",
        "Mechanic",
        "PartyGirl",
        "Wizard",
        "TaxCollector",
        "Truffle",
        "Pirate",
        "Steampunker",
        "Cyborg",
        "SantaClaus",
        "Princess",
        "TravellingMerchant",
        "TravelingMerchant",
        "SkeletonMerchant",
        "OldMan",
        "TownCat",
        "TownDog",
        "TownBunny",
        "TownSlimeBlue",
        "TownSlimeCopper",
        "TownSlimeGreen",
        "TownSlimeOld",
        "TownSlimePurple",
        "TownSlimeRainbow",
        "TownSlimeRed",
        "TownSlimeYellow"
    ];

    public static readonly IReadOnlyList<string> AlwaysIncludeMerchantNpcIds = ["OldMan"];
    public static readonly IReadOnlyList<string> DownedFlagPrefixes = ["downed"];
    public static readonly IReadOnlyList<string> MainFlags = ["Main.hardMode"];

    public static string NormalizeTheme(string? value)
    {
        var trimmed = (value ?? string.Empty).Trim();
        if (string.Equals(trimmed, ThemeSystem, StringComparison.OrdinalIgnoreCase))
            return ThemeSystem;
        if (string.Equals(trimmed, ThemeLight, StringComparison.OrdinalIgnoreCase))
            return ThemeLight;
        return ThemeDark;
    }
}
