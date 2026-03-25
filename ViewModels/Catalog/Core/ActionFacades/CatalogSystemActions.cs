namespace MerchantsPlus.Generator;

public abstract class CatalogSystemActions : CatalogMutationActions
{
    protected CatalogSystemActions() { }

    protected CatalogSystemActions(
        CatalogCategoryActionsComponent categoryActions,
        CatalogShopActionsComponent shopActions,
        CatalogRuleActionsComponent ruleActions,
        CatalogSettingsActionsComponent settingsActions,
        CatalogOutputActionsComponent outputActions)
        : base(categoryActions, shopActions, ruleActions, settingsActions, outputActions) { }

    public void RebuildCatalog(bool forceRefresh = false) => TryRebuildCatalog(forceRefresh, restoreSelectionAfterReload: false, showSuccessStatus: true, failurePrefix: "Rebuild failed");

    protected override void AutoRebuildCatalog()
    {
        TryRebuildCatalog(forceRefresh: false, restoreSelectionAfterReload: true, showSuccessStatus: false, failurePrefix: "Auto rebuild failed");
    }

    public void SaveUnorganizedBlacklistSettings()
    {
        ItemCatalog.UpdateUnorganizedBlacklistConfig(CollectRules(UnorganizedKeywordRules));
        LoadUnorganizedBlacklistInputs();
        AutoRebuildCatalog();
        EditorStatus = "Saved unorganized blacklist rules and rebuilt catalog.";
    }

    public void ClearUnorganizedBlacklistSettings()
    {
        UnorganizedKeywordRules.Clear();
        NewUnorganizedRuleTerm = string.Empty;
        SaveUnorganizedBlacklistSettings();
    }

    public void LoadSettings()
    {
        var loaded = SettingsActions.LoadSettings();
        _isSettingsLoading = true;
        TModLoaderDllPath = loaded.TModLoaderDllPath;
        AllowedMerchantNpcIdsText = loaded.AllowedMerchantNpcIdsText;
        AlwaysIncludeMerchantNpcIdsText = loaded.AlwaysIncludeMerchantNpcIdsText;
        DownedFlagPrefixesText = loaded.DownedFlagPrefixesText;
        MainFlagsText = loaded.MainFlagsText;
        AppTextScale = loaded.AppTextScale;
        TreeViewTextScale = loaded.TreeViewTextScale;
        AppTheme = loaded.AppTheme;
        _isSettingsLoading = false;
        UpdateTModLoaderDllPathStatus();
    }

    public void SaveSettings(bool showStatus = true)
    {
        SettingsActions.SaveSettings(BuildSettingsConfig());
        if (showStatus)
            EditorStatus = SettingsActions.BuildSaveStatus(IsTModLoaderDllPathValid);
    }

    public void AutoDetectTModLoaderDllPath()
    {
        var detected = SettingsActions.AutoDetectTModLoaderDllPath();
        if (string.IsNullOrWhiteSpace(detected))
        {
            EditorStatus = "Auto-detect failed. Provide the tModLoader.dll path manually.";
            return;
        }

        TModLoaderDllPath = detected;
        EditorStatus = "Auto-detected tModLoader.dll path.";
    }

    public void RefreshOutputFiles()
    {
        var selected = SelectedOutputFile;
        OutputFiles.Clear();
        foreach (var file in OutputActions.ListOutputFiles())
            OutputFiles.Add(file);

        if (OutputFiles.Count == 0)
        {
            SelectedOutputFile = string.Empty;
            OutputFileContents = string.Empty;
            return;
        }

        SelectedOutputFile = !string.IsNullOrWhiteSpace(selected) && OutputFiles.Contains(selected) ? selected : OutputFiles[0];
    }

    public void OpenOutputFolder()
    {
        try { OutputActions.OpenOutputFolder(); }
        catch (Exception ex) { EditorStatus = $"Failed to open output folder: {ex.Message}"; }
    }

    public void GenerateCatalogDataScript()
    {
        var result = OutputActions.GenerateCatalogDataScript(className: "GeneratedCatalogData", overwrite: true);
        EditorStatus = result.Message;
    }

    protected override void OnSettingsChanged()
    {
        if (!_isSettingsLoading)
            SaveSettings(showStatus: false);
    }

    protected override void OnSelectedOutputFileChanged() => OutputFileContents = OutputActions.LoadOutputFileContents(SelectedOutputFile);

    private void TryRebuildCatalog(bool forceRefresh, bool restoreSelectionAfterReload, bool showSuccessStatus, string failurePrefix)
    {
        try
        {
            var expandedState = CatalogTreeState.CaptureExpandedState(RootNodes);
            var selectedCategory = SelectedCategoryForShop;
            var selectedShop = SelectedShopName;
            var selectedMerchant = SelectedMerchantName;
            var preserveUnorganizedSelection = IsUnorganizedSelectionActive();

            ItemCatalog.BuildCatalog(forceRefresh);
            Load(ItemCatalog.Categories, ItemCatalog.Unorganized);
            if (preserveUnorganizedSelection)
                RestoreUnorganizedSelection();
            else if (restoreSelectionAfterReload)
                RestoreSelectionAfterReload(selectedCategory, selectedShop);

            CatalogTreeState.ApplyExpandedState(RootNodes, expandedState);

            if (!string.IsNullOrWhiteSpace(selectedMerchant) && MerchantNpcIds.Contains(selectedMerchant))
                SelectedMerchantName = selectedMerchant;

            RefreshBlacklistedUnorganizedItems();
            if (showSuccessStatus)
                EditorStatus = "Catalog rebuilt successfully.";
        }
        catch (Exception ex)
        {
            EditorStatus = $"{failurePrefix}: {ex.Message}";
        }
    }

    private bool IsUnorganizedSelectionActive()
    {
        return SelectedNode?.Type == CatalogNodeType.Group
            && SelectedNode.Title.StartsWith("Unorganized", StringComparison.OrdinalIgnoreCase);
    }

    private void RestoreUnorganizedSelection()
    {
        var unorganizedNode = RootNodes.FirstOrDefault(node =>
            node.Type == CatalogNodeType.Group
            && node.Title.StartsWith("Unorganized", StringComparison.OrdinalIgnoreCase));
        if (unorganizedNode is not null)
            SelectedNode = unorganizedNode;
    }
}
