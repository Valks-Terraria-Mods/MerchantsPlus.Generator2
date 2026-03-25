namespace MerchantsPlus.Generator;

public abstract class CatalogActionComponentsHost : CatalogFilterState
{
    private readonly CatalogCategoryActionsComponent _categoryActions;
    private readonly CatalogShopActionsComponent _shopActions;
    private readonly CatalogRuleActionsComponent _ruleActions;
    private readonly CatalogSettingsActionsComponent _settingsActions;
    private readonly CatalogOutputActionsComponent _outputActions;

    protected CatalogActionComponentsHost()
        : this(new CatalogCategoryActionsComponent(), new CatalogShopActionsComponent(), new CatalogRuleActionsComponent(), new CatalogSettingsActionsComponent(), new CatalogOutputActionsComponent()) { }

    protected CatalogActionComponentsHost(
        CatalogCategoryActionsComponent categoryActions,
        CatalogShopActionsComponent shopActions,
        CatalogRuleActionsComponent ruleActions,
        CatalogSettingsActionsComponent settingsActions,
        CatalogOutputActionsComponent outputActions)
    {
        _categoryActions = categoryActions;
        _shopActions = shopActions;
        _ruleActions = ruleActions;
        _settingsActions = settingsActions;
        _outputActions = outputActions;
    }

    protected CatalogCategoryActionsComponent CategoryActions => _categoryActions;
    protected CatalogShopActionsComponent ShopActions => _shopActions;
    protected CatalogRuleActionsComponent RuleActions => _ruleActions;
    protected CatalogSettingsActionsComponent SettingsActions => _settingsActions;
    protected CatalogOutputActionsComponent OutputActions => _outputActions;

    protected abstract void Load(IReadOnlyList<Category> categories, IReadOnlyList<string> unorganized);
    protected abstract void RefreshCategoryOptions();
    protected abstract void RefreshShopOptions();
    protected abstract void ReloadFromCatalogPreservingSelection();
    protected abstract void RestoreSelectionAfterReload(string selectedCategory, string selectedShop);
}
