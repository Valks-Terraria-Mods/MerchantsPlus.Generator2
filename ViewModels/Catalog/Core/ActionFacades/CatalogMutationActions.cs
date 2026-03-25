namespace MerchantsPlus.Generator;

public abstract class CatalogMutationActions : CatalogActionComponentsHost
{
    protected CatalogMutationActions() { }

    protected CatalogMutationActions(
        CatalogCategoryActionsComponent categoryActions,
        CatalogShopActionsComponent shopActions,
        CatalogRuleActionsComponent ruleActions,
        CatalogSettingsActionsComponent settingsActions,
        CatalogOutputActionsComponent outputActions)
        : base(categoryActions, shopActions, ruleActions, settingsActions, outputActions) { }

    public void InitializeUnorganizedBlacklistEditor() => LoadUnorganizedBlacklistInputs();

    public void AddCategory()
    {
        if (!CategoryActions.TryAddCategory(NewCategoryName, CollectRules(CategoryKeywordRules), ParseCommaSeparated(NewCategoryFirstOrder), ParseCommaSeparated(NewCategoryLastOrder), out var status))
        {
            EditorStatus = status;
            return;
        }

        ResetCategoryInputs();
        EditorStatus = status;
        RefreshCategoryOptions();
        AutoRebuildCatalog();
    }

    public void UpdateSelectedCategory()
    {
        if (!CategoryActions.TryUpdateCategory(SelectedCategoryForShop, NewCategoryName, CollectRules(CategoryKeywordRules), ParseCommaSeparated(NewCategoryFirstOrder), ParseCommaSeparated(NewCategoryLastOrder), out var resolvedCategoryName, out var status))
        {
            EditorStatus = status;
            return;
        }

        EditorStatus = status;
        RefreshCategoryOptions();
        SelectedCategoryForShop = resolvedCategoryName;
        ReloadFromCatalogPreservingSelection();
        AutoRebuildCatalog();
    }

    public void RemoveSelectedCategory()
    {
        if (!CategoryActions.TryRemoveCategory(SelectedCategoryForShop, out var status))
        {
            EditorStatus = status;
            return;
        }

        ResetCategoryInputs();
        EditorStatus = status;
        RefreshCategoryOptions();
        ReloadFromCatalogPreservingSelection();
        AutoRebuildCatalog();
    }

    public void SaveShop()
    {
        if (!ShopActions.TrySaveShop(SelectedCategoryForShop, SelectedShopName, NewShopName, NewShopPriority, CollectRules(ShopKeywordRules), ParseCommaSeparated(NewShopFirstOrder), ParseCommaSeparated(NewShopLastOrder), out var savedShopName, out var status))
        {
            EditorStatus = status;
            return;
        }

        EditorStatus = status;
        RefreshShopOptions();
        SelectedShopName = savedShopName;
        ReloadFromCatalogPreservingSelection();
        AutoRebuildCatalog();
    }

    public void RemoveSelectedShop()
    {
        if (!ShopActions.TryRemoveShop(SelectedCategoryForShop, SelectedShopName, out var status))
        {
            EditorStatus = status;
            return;
        }

        EditorStatus = status;
        SelectedShopName = string.Empty;
        ClearShopInputs();
        RefreshShopOptions();
        ReloadFromCatalogPreservingSelection();
        AutoRebuildCatalog();
    }

    public void MoveSelectedShop(string targetCategoryName, string? targetParentShopName)
    {
        var sourceCategory = SelectedCategoryForShop;
        var selectedShop = SelectedShopName;
        if (!ShopActions.TryMoveShop(sourceCategory, selectedShop, targetCategoryName, targetParentShopName, out var status))
        {
            EditorStatus = status;
            return;
        }

        EditorStatus = status;
        ReloadFromCatalogPreservingSelection();
        // Keep focus anchored to the original category instead of jumping to the destination.
        SelectedShopName = string.Empty;
        AutoRebuildCatalog();
    }

    public void BeginNewShop()
    {
        SelectedShopName = string.Empty;
        ClearShopInputs();
        EditorStatus = "Enter a new shop name, then click Save Shop.";
    }

    public void ExcludeItem(string itemName)
    {
        if (!ShopActions.TryAddExcludedItemRule(SelectedCategoryForShop, SelectedShopName, itemName, ShopKeywordRules, out var status))
        {
            EditorStatus = status;
            return;
        }

        SaveShop();
        EditorStatus = status;
    }

    public void AddCategoryKeywordRule() => RuleActions.AddRule(CategoryKeywordRules, NewCategoryRuleMode, NewCategoryRuleMatch, NewCategoryRuleTerm, value => NewCategoryRuleTerm = value);
    public void RemoveCategoryKeywordRule(KeywordRuleEditorRow rule) => CategoryKeywordRules.Remove(rule);
    public void AddShopKeywordRule() => RuleActions.AddRule(ShopKeywordRules, NewShopRuleMode, NewShopRuleMatch, NewShopRuleTerm, value => NewShopRuleTerm = value);
    public void RemoveShopKeywordRule(KeywordRuleEditorRow rule) => ShopKeywordRules.Remove(rule);
    public void AddUnorganizedKeywordRule() => RuleActions.AddRule(UnorganizedKeywordRules, NewUnorganizedRuleMode, NewUnorganizedRuleMatch, NewUnorganizedRuleTerm, value => NewUnorganizedRuleTerm = value);
    public void RemoveUnorganizedKeywordRule(KeywordRuleEditorRow rule) => UnorganizedKeywordRules.Remove(rule);

    protected abstract void AutoRebuildCatalog();

    private void ResetCategoryInputs()
    {
        NewCategoryName = string.Empty;
        CategoryKeywordRules.Clear();
        NewCategoryRuleTerm = string.Empty;
        NewCategoryFirstOrder = string.Empty;
        NewCategoryLastOrder = string.Empty;
    }
}
