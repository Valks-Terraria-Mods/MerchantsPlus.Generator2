namespace MerchantsPlus.Generator;

public class CatalogShopService{
    private readonly ItemCatalogState _state;
    private readonly CatalogConfigurationSupport _configurationSupport;
    private readonly CatalogShopTreeService _shopTreeService;

    public CatalogShopService(
        ItemCatalogState state,
        CatalogConfigurationSupport configurationSupport,
        CatalogShopTreeService shopTreeService)
    {
        _state = state;
        _configurationSupport = configurationSupport;
        _shopTreeService = shopTreeService;
    }

    public bool AddShop(string categoryName, Shop shop)
    {
        var category = FindCategory(categoryName);
        if (category is null || HasShopNamed(category, shop.Name))
            return false;

        CatalogShopMutationHelper.NormalizeShopRules(shop);
        category.Shops.Add(shop);

        _configurationSupport.ApplyCategoryAndSharedOrderLinks(category);
        _configurationSupport.SaveConfiguration();
        return true;
    }

    public bool UpsertShop(string categoryName, string? existingShopName, Shop shop)
    {
        var category = FindCategory(categoryName);
        if (category is null)
            return false;

        if (!string.IsNullOrWhiteSpace(existingShopName))
            return UpdateExistingShop(category, existingShopName, shop);

        if (HasShopNamed(category, shop.Name))
            return false;

        CatalogShopMutationHelper.NormalizeShopRules(shop);
        category.Shops.Add(shop);

        _configurationSupport.ApplyCategoryAndSharedOrderLinks(category);
        _configurationSupport.SaveConfiguration();
        return true;
    }

    public bool RemoveShop(string categoryName, string shopName)
    {
        var category = FindCategory(categoryName);
        if (category is null)
            return false;

        var shop = FindShop(category, shopName);
        if (shop is null)
            return false;

        CatalogShopMutationHelper.ResetOrderDependents(category, shop.Name);
        CatalogShopMutationHelper.ClearChildParentLinks(category, shop.Name);

        category.Shops.Remove(shop);

        _configurationSupport.ApplyCategoryAndSharedOrderLinks(category);
        _configurationSupport.SaveConfiguration();
        return true;
    }

    public bool MoveShop(string sourceCategoryName, string shopName, string targetCategoryName, string? targetParentShopName)
    {
        var sourceCategory = FindCategory(sourceCategoryName);
        var targetCategory = FindCategory(targetCategoryName);
        if (sourceCategory is null || targetCategory is null)
            return false;

        var shop = FindShop(sourceCategory, shopName);
        if (shop is null)
            return false;

        var normalizedTargetParent = targetParentShopName?.Trim() ?? string.Empty;
        var sourceAndTargetAreSame = ReferenceEquals(sourceCategory, targetCategory);
        var subtree = _shopTreeService.CollectShopSubtree(sourceCategory, shop);
        var subtreeNames = subtree.Select(candidate => candidate.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (sourceAndTargetAreSame && string.Equals(shop.ParentShopName, normalizedTargetParent, StringComparison.OrdinalIgnoreCase))
            return false;

        if (!CatalogShopMutationHelper.IsTargetParentValid(targetCategory, normalizedTargetParent, subtreeNames))
            return false;

        if (!sourceAndTargetAreSame && targetCategory.Shops.Any(candidate => subtreeNames.Contains(candidate.Name)))
            return false;

        CatalogShopMutationHelper.ResetOrderDependents(sourceCategory, shop.Name);

        if (!sourceAndTargetAreSame)
        {
            foreach (var movedShop in subtree)
                sourceCategory.Shops.Remove(movedShop);

            shop.SharedOrderSourceShopName = string.Empty;
            foreach (var movedShop in subtree)
                targetCategory.Shops.Add(movedShop);
        }

        shop.ParentShopName = normalizedTargetParent;

        _configurationSupport.ApplyCategoryAndSharedOrderLinks(sourceCategory);
        if (!ReferenceEquals(sourceCategory, targetCategory))
            _configurationSupport.ApplyCategoryAndSharedOrderLinks(targetCategory);

        _configurationSupport.SaveConfiguration();
        return true;
    }

    private Category? FindCategory(string categoryName)
    {
        return _state.Categories.FirstOrDefault(category => string.Equals(category.Name, categoryName, StringComparison.OrdinalIgnoreCase));
    }

    private static Shop? FindShop(Category category, string shopName)
    {
        return category.Shops.FirstOrDefault(shop => string.Equals(shop.Name, shopName, StringComparison.OrdinalIgnoreCase));
    }

    private static bool HasShopNamed(Category category, string shopName)
    {
        return CatalogShopMutationHelper.HasShopNamed(category, shopName);
    }

    private bool UpdateExistingShop(Category category, string existingShopName, Shop shop)
    {
        var existingShop = FindShop(category, existingShopName);
        if (existingShop is null)
            return false;

        if (category.Shops.Any(candidate => !ReferenceEquals(candidate, existingShop)
            && string.Equals(candidate.Name, shop.Name, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var oldName = existingShop.Name;
        existingShop.Name = shop.Name;
        existingShop.Priority = shop.Priority;
        existingShop.KeywordRules = KeywordRuleSet.Normalize(shop.KeywordRules);
        existingShop.Keywords = KeywordRuleSet.ToLegacyContains(existingShop.KeywordRules, KeywordRule.WhitelistMode);
        existingShop.FirstOrder = shop.FirstOrder;
        existingShop.LastOrder = shop.LastOrder;
        existingShop.ExcludedItems = KeywordRuleSet.ToLegacyContains(existingShop.KeywordRules, KeywordRule.BlacklistMode);
        existingShop.SharedOrderSourceShopName = shop.SharedOrderSourceShopName;
        existingShop.ParentShopName = shop.ParentShopName?.Trim() ?? string.Empty;

        if (!string.Equals(oldName, existingShop.Name, StringComparison.OrdinalIgnoreCase))
            _configurationSupport.UpdateDependentSourceNames(category, oldName, existingShop.Name);

        _configurationSupport.ApplyCategoryAndSharedOrderLinks(category);
        _configurationSupport.SaveConfiguration();
        return true;
    }

}