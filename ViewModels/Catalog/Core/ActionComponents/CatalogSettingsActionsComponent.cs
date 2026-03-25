namespace MerchantsPlus.Generator;

public sealed class CatalogSettingsActionsComponent{
    public CatalogSettingsLoadResult LoadSettings()
    {
        var config = CatalogStorage.LoadSettingsConfig();
        return new CatalogSettingsLoadResult(
            config.TModLoaderDllPath ?? string.Empty,
            string.Join(", ", config.AllowedMerchantNpcIds is { Length: > 0 } ? config.AllowedMerchantNpcIds : SettingsDefaults.AllowedMerchantNpcIds),
            string.Join(", ", config.AlwaysIncludeMerchantNpcIds is { Length: > 0 } ? config.AlwaysIncludeMerchantNpcIds : SettingsDefaults.AlwaysIncludeMerchantNpcIds),
            string.Join(", ", config.DownedFlagPrefixes is { Length: > 0 } ? config.DownedFlagPrefixes : SettingsDefaults.DownedFlagPrefixes),
            string.Join(", ", config.MainFlags is { Length: > 0 } ? config.MainFlags : SettingsDefaults.MainFlags),
            NormalizeTextScale(config.AppTextScale, SettingsDefaults.AppTextScaleDefault),
            NormalizeTextScale(config.TreeViewTextScale, SettingsDefaults.TreeViewTextScaleDefault),
            SettingsDefaults.NormalizeTheme(config.AppTheme));
    }

    public void SaveSettings(SettingsConfig config)
    {
        CatalogStorage.SaveSettingsConfig(config);
    }

    public string BuildSaveStatus(bool isTModLoaderDllPathValid)
    {
        return isTModLoaderDllPathValid
            ? "Saved settings. tModLoader.dll path is valid."
            : "Saved settings. tModLoader.dll path does not exist.";
    }

    public string? AutoDetectTModLoaderDllPath()
    {
        return TModLoaderLocator.FindTModLoaderDllPath();
    }

    private static double NormalizeTextScale(double value, double fallback)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0)
            return fallback;

        return Math.Clamp(value, SettingsDefaults.TextScaleMin, SettingsDefaults.TextScaleMax);
    }
}
