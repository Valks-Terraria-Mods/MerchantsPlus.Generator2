namespace MerchantsPlus.Generator;

public sealed class CatalogSettingsLoadResult{
    private readonly string _tModLoaderDllPath;
    private readonly string _allowedMerchantNpcIdsText;
    private readonly string _alwaysIncludeMerchantNpcIdsText;
    private readonly string _downedFlagPrefixesText;
    private readonly string _mainFlagsText;
    private readonly double _appTextScale;
    private readonly double _treeViewTextScale;
    private readonly string _appTheme;

    public CatalogSettingsLoadResult(
        string tModLoaderDllPath,
        string allowedMerchantNpcIdsText,
        string alwaysIncludeMerchantNpcIdsText,
        string downedFlagPrefixesText,
        string mainFlagsText,
        double appTextScale,
        double treeViewTextScale,
        string appTheme)
    {
        _tModLoaderDllPath = tModLoaderDllPath;
        _allowedMerchantNpcIdsText = allowedMerchantNpcIdsText;
        _alwaysIncludeMerchantNpcIdsText = alwaysIncludeMerchantNpcIdsText;
        _downedFlagPrefixesText = downedFlagPrefixesText;
        _mainFlagsText = mainFlagsText;
        _appTextScale = appTextScale;
        _treeViewTextScale = treeViewTextScale;
        _appTheme = appTheme;
    }

    public string TModLoaderDllPath => _tModLoaderDllPath;
    public string AllowedMerchantNpcIdsText => _allowedMerchantNpcIdsText;
    public string AlwaysIncludeMerchantNpcIdsText => _alwaysIncludeMerchantNpcIdsText;
    public string DownedFlagPrefixesText => _downedFlagPrefixesText;
    public string MainFlagsText => _mainFlagsText;
    public double AppTextScale => _appTextScale;
    public double TreeViewTextScale => _treeViewTextScale;
    public string AppTheme => _appTheme;
}
