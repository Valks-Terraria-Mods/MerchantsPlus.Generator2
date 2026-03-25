namespace MerchantsPlus.Generator;

public sealed class SettingsConfig{
    public string TModLoaderDllPath { get; set; } = string.Empty;
    public string[] AllowedMerchantNpcIds { get; set; } = [];
    public string[] AlwaysIncludeMerchantNpcIds { get; set; } = [];
    public string[] DownedFlagPrefixes { get; set; } = [];
    public string[] MainFlags { get; set; } = [];
    public double AppTextScale { get; set; } = SettingsDefaults.AppTextScaleDefault;
    public double TreeViewTextScale { get; set; } = SettingsDefaults.TreeViewTextScaleDefault;
    public string AppTheme { get; set; } = SettingsDefaults.AppThemeDefault;
}
