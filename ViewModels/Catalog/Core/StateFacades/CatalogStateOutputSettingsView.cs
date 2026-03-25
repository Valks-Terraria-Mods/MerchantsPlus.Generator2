namespace MerchantsPlus.Generator;
public abstract class CatalogStateOutputSettingsView : CatalogStateComponentHost
{
    protected CatalogStateOutputSettingsView() { }
    protected CatalogStateOutputSettingsView(CatalogStateComponents components) : base(components) { }
    public int WorkspaceTabIndex
    {
        get => Selection.WorkspaceTabIndex;
        set
        {
            if (!SetComponentField(Selection.WorkspaceTabIndex, value, v => Selection.WorkspaceTabIndex = v))
                return;
            OnPropertyChanged(nameof(IsItemsTabVisible));
            OnPropertyChanged(nameof(IsMerchantsTabVisible));
            OnPropertyChanged(nameof(IsOutputTabVisible));
            OnPropertyChanged(nameof(IsStatsTabVisible));
        }
    }

    public bool IsItemsTabVisible => Selection.WorkspaceTabIndex == 0;
    public bool IsMerchantsTabVisible => Selection.WorkspaceTabIndex == 1;
    public bool IsOutputTabVisible => Selection.WorkspaceTabIndex == 2;
    public bool IsStatsTabVisible => Selection.WorkspaceTabIndex == 3;
    public string EditorStatus { get => Selection.EditorStatus; set => SetComponentField(Selection.EditorStatus, value, v => Selection.EditorStatus = v); }
    public string MerchantAssignmentHint { get => Selection.MerchantAssignmentHint; set => SetComponentField(Selection.MerchantAssignmentHint, value, v => Selection.MerchantAssignmentHint = v); }
    public int TotalCategoryCount => ItemCatalog.Categories.Count;
    public int TotalShopCount => ItemCatalog.Categories.Sum(category => category.Shops.Count);
    public int TotalItemCount => ItemCatalog.Categories.Sum(category => category.Shops.Sum(shop => shop.Items.Count));
    public int TotalMerchantCount => MerchantNpcIds.Count;
    public int TotalUnorganizedItemCount => ItemCatalog.Unorganized.Count;
    public virtual int TotalAssignedShopCount => 0;
    public virtual int TotalUnassignedShopCount => 0;
    public string SelectedOutputFile
    {
        get => Output.SelectedOutputFile;
        set
        {
            if (!SetComponentField(Output.SelectedOutputFile, value, v => Output.SelectedOutputFile = v))
                return;
            OnSelectedOutputFileChanged();
        }
    }

    public string OutputFileContents { get => Output.OutputFileContents; set => SetComponentField(Output.OutputFileContents, value, v => Output.OutputFileContents = v); }
    public string OutputSearchText { get => Output.OutputSearchText; set => SetComponentField(Output.OutputSearchText, value, v => Output.OutputSearchText = v); }

    public int OutputSearchMatchCount
    {
        get => Output.OutputSearchMatchCount;
        set
        {
            if (!SetComponentField(Output.OutputSearchMatchCount, value, v => Output.OutputSearchMatchCount = v))
                return;
            OnPropertyChanged(nameof(OutputSearchStatus));
        }
    }

    public int OutputSearchMatchIndex
    {
        get => Output.OutputSearchMatchIndex;
        set
        {
            if (!SetComponentField(Output.OutputSearchMatchIndex, value, v => Output.OutputSearchMatchIndex = v))
                return;
            OnPropertyChanged(nameof(OutputSearchStatus));
        }
    }

    public bool CanOutputSearchPrev { get => Output.CanOutputSearchPrev; set => SetComponentField(Output.CanOutputSearchPrev, value, v => Output.CanOutputSearchPrev = v); }
    public bool CanOutputSearchNext { get => Output.CanOutputSearchNext; set => SetComponentField(Output.CanOutputSearchNext, value, v => Output.CanOutputSearchNext = v); }
    public string OutputSearchStatus => OutputSearchMatchCount == 0 ? "0 matches" : $"{OutputSearchMatchIndex} of {OutputSearchMatchCount}";

    public string AllowedMerchantNpcIdsText
    {
        get => Settings.AllowedMerchantNpcIdsText;
        set
        {
            if (!SetComponentField(Settings.AllowedMerchantNpcIdsText, value, v => Settings.AllowedMerchantNpcIdsText = v))
                return;
            OnSettingsChanged();
        }
    }

    public string AlwaysIncludeMerchantNpcIdsText
    {
        get => Settings.AlwaysIncludeMerchantNpcIdsText;
        set
        {
            if (!SetComponentField(Settings.AlwaysIncludeMerchantNpcIdsText, value, v => Settings.AlwaysIncludeMerchantNpcIdsText = v))
                return;
            OnSettingsChanged();
        }
    }

    public string DownedFlagPrefixesText
    {
        get => Settings.DownedFlagPrefixesText;
        set
        {
            if (!SetComponentField(Settings.DownedFlagPrefixesText, value, v => Settings.DownedFlagPrefixesText = v))
                return;
            OnSettingsChanged();
        }
    }

    public string MainFlagsText
    {
        get => Settings.MainFlagsText;
        set
        {
            if (!SetComponentField(Settings.MainFlagsText, value, v => Settings.MainFlagsText = v))
                return;
            OnSettingsChanged();
        }
    }

    public string TModLoaderDllPath
    {
        get => Settings.TModLoaderDllPath;
        set
        {
            if (string.Equals(Settings.TModLoaderDllPath, value, StringComparison.Ordinal))
            {
                UpdateTModLoaderDllPathStatus();
                OnSettingsChanged();
                return;
            }

            Settings.TModLoaderDllPath = value;
            OnPropertyChanged();
            UpdateTModLoaderDllPathStatus();
            OnSettingsChanged();
        }
    }

    public string TModLoaderDllPathStatus
    {
        get => Settings.TModLoaderDllPathStatus;
        protected set => SetComponentField(Settings.TModLoaderDllPathStatus, value, v => Settings.TModLoaderDllPathStatus = v);
    }

    public bool IsTModLoaderDllPathValid
    {
        get => Settings.IsTModLoaderDllPathValid;
        protected set => SetComponentField(Settings.IsTModLoaderDllPathValid, value, v => Settings.IsTModLoaderDllPathValid = v);
    }

    public int SettingsTabIndex { get => Settings.SettingsTabIndex; set => SetComponentField(Settings.SettingsTabIndex, value, v => Settings.SettingsTabIndex = v); }

    public double AppTextScale
    {
        get => Settings.AppTextScale;
        set => SetTextScale(value, Settings.AppTextScale, v => Settings.AppTextScale = v, nameof(AppTextScale), nameof(AppFontSize), true);
    }

    public double TreeViewTextScale
    {
        get => Settings.TreeViewTextScale;
        set => SetTextScale(value, Settings.TreeViewTextScale, v => Settings.TreeViewTextScale = v, nameof(TreeViewTextScale), nameof(TreeViewFontSize), false);
    }

    public double AppFontSize => SettingsDefaults.BaseAppFontSize * AppTextScale;
    public double TreeViewFontSize => SettingsDefaults.BaseTreeViewFontSize * TreeViewTextScale;

    public string MerchantSortMode
    {
        get => Selection.MerchantSortMode;
        set
        {
            if (!SetComponentField(Selection.MerchantSortMode, value, v => Selection.MerchantSortMode = v))
                return;
            OnMerchantSortChanged();
        }
    }

    protected void NotifyWorkspaceStatsChanged()
    {
        OnPropertyChanged(nameof(TotalCategoryCount));
        OnPropertyChanged(nameof(TotalShopCount));
        OnPropertyChanged(nameof(TotalItemCount));
        OnPropertyChanged(nameof(TotalMerchantCount));
        OnPropertyChanged(nameof(TotalUnorganizedItemCount));
        OnPropertyChanged(nameof(TotalAssignedShopCount));
        OnPropertyChanged(nameof(TotalUnassignedShopCount));
    }
    private void SetTextScale(double value, double current, Action<double> setter, string scalePropertyName, string fontSizePropertyName, bool notifyGlobalFontSizes)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            return;
        var clamped = Math.Clamp(value, SettingsDefaults.TextScaleMin, SettingsDefaults.TextScaleMax);
        if (!SetComponentField(current, clamped, setter, scalePropertyName))
            return;
        OnPropertyChanged(fontSizePropertyName);
        if (notifyGlobalFontSizes)
            NotifyAppFontSizesChanged();
        OnSettingsChanged();
    }

    protected abstract void UpdateTModLoaderDllPathStatus();
}
