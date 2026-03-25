using System.Text.Json;

namespace MerchantsPlus.Generator;

internal static class StorageSettingsComponent
{
    private const string SettingsFileName = "settings.json";

    internal static SettingsConfig LoadSettingsConfig(string settingsDirectory, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(settingsDirectory);
        var path = Path.Combine(settingsDirectory, SettingsFileName);

        if (!File.Exists(path))
            return new SettingsConfig();

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<SettingsConfig>(json, options);
        if (data is null)
            return new SettingsConfig();

        data.AppTextScale = NormalizeTextScale(data.AppTextScale, SettingsDefaults.AppTextScaleDefault);
        data.TreeViewTextScale = NormalizeTextScale(data.TreeViewTextScale, SettingsDefaults.TreeViewTextScaleDefault);
        data.AppTheme = SettingsDefaults.NormalizeTheme(data.AppTheme);
        return data;
    }

    internal static void SaveSettingsConfig(string settingsDirectory, SettingsConfig config, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(settingsDirectory);
        var path = Path.Combine(settingsDirectory, SettingsFileName);

        var stable = new SettingsConfig
        {
            TModLoaderDllPath = config.TModLoaderDllPath?.Trim() ?? string.Empty,
            AllowedMerchantNpcIds = config.AllowedMerchantNpcIds ?? [],
            AlwaysIncludeMerchantNpcIds = config.AlwaysIncludeMerchantNpcIds ?? [],
            DownedFlagPrefixes = config.DownedFlagPrefixes ?? [],
            MainFlags = config.MainFlags ?? [],
            AppTextScale = NormalizeTextScale(config.AppTextScale, SettingsDefaults.AppTextScaleDefault),
            TreeViewTextScale = NormalizeTextScale(config.TreeViewTextScale, SettingsDefaults.TreeViewTextScaleDefault),
            AppTheme = SettingsDefaults.NormalizeTheme(config.AppTheme)
        };

        var json = JsonSerializer.Serialize(stable, options);
        json = StoragePathHelper.ConvertToFourSpaceIndent(json);
        File.WriteAllText(path, json);
    }

    private static double NormalizeTextScale(double value, double fallback)
    {
        if (double.IsNaN(value) || double.IsInfinity(value) || value <= 0)
            return fallback;

        return Math.Clamp(value, SettingsDefaults.TextScaleMin, SettingsDefaults.TextScaleMax);
    }
}
