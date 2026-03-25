namespace MerchantsPlus.Generator;

public abstract class CatalogActions : CatalogSystemActions
{
    protected CatalogActions() { }

    protected CatalogActions(
        CatalogCategoryActionsComponent categoryActions,
        CatalogShopActionsComponent shopActions,
        CatalogRuleActionsComponent ruleActions,
        CatalogSettingsActionsComponent settingsActions,
        CatalogOutputActionsComponent outputActions)
        : base(categoryActions, shopActions, ruleActions, settingsActions, outputActions) { }
}
