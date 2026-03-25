namespace MerchantsPlus.Generator;

public sealed class CatalogSettingsComponent{
    private string _tmodLoaderDllPath = string.Empty;
    private string _tmodLoaderDllPathStatus = "No path set.";
    private bool _isTModLoaderDllPathValid;
    private int _settingsTabIndex;
    private string _allowedMerchantNpcIdsText = string.Empty;
    private string _alwaysIncludeMerchantNpcIdsText = string.Empty;
    private string _downedFlagPrefixesText = string.Empty;
    private string _mainFlagsText = string.Empty;
    private double _appTextScale = SettingsDefaults.AppTextScaleDefault;
    private double _treeViewTextScale = SettingsDefaults.TreeViewTextScaleDefault;
    private string _appTheme = SettingsDefaults.AppThemeDefault;
    private bool _isSettingsLoading;

    public string TModLoaderDllPath { get => _tmodLoaderDllPath; set => _tmodLoaderDllPath = value; }
    public string TModLoaderDllPathStatus { get => _tmodLoaderDllPathStatus; set => _tmodLoaderDllPathStatus = value; }
    public bool IsTModLoaderDllPathValid { get => _isTModLoaderDllPathValid; set => _isTModLoaderDllPathValid = value; }
    public int SettingsTabIndex { get => _settingsTabIndex; set => _settingsTabIndex = value; }
    public string AllowedMerchantNpcIdsText { get => _allowedMerchantNpcIdsText; set => _allowedMerchantNpcIdsText = value; }
    public string AlwaysIncludeMerchantNpcIdsText { get => _alwaysIncludeMerchantNpcIdsText; set => _alwaysIncludeMerchantNpcIdsText = value; }
    public string DownedFlagPrefixesText { get => _downedFlagPrefixesText; set => _downedFlagPrefixesText = value; }
    public string MainFlagsText { get => _mainFlagsText; set => _mainFlagsText = value; }
    public double AppTextScale { get => _appTextScale; set => _appTextScale = value; }
    public double TreeViewTextScale { get => _treeViewTextScale; set => _treeViewTextScale = value; }
    public string AppTheme { get => _appTheme; set => _appTheme = value; }
    public bool IsSettingsLoading { get => _isSettingsLoading; set => _isSettingsLoading = value; }
}
