namespace MerchantsPlus.Generator;

public static class CatalogShopMutationHelper
{
    public static bool HasShopNamed(Category category, string shopName)
    {
        return category.Shops.Any(shop => string.Equals(shop.Name, shopName, StringComparison.OrdinalIgnoreCase));
    }

    public static void NormalizeShopRules(Shop shop)
    {
        shop.KeywordRules = KeywordRuleSet.Normalize(shop.KeywordRules);
        shop.Keywords = KeywordRuleSet.ToLegacyContains(shop.KeywordRules, KeywordRule.WhitelistMode);
        shop.ExcludedItems = KeywordRuleSet.ToLegacyContains(shop.KeywordRules, KeywordRule.BlacklistMode);
        shop.ParentShopName = shop.ParentShopName?.Trim() ?? string.Empty;
    }

    public static void ResetOrderDependents(Category category, string sourceShopName)
    {
        var dependents = category.Shops.Where(shop => string.Equals(shop.SharedOrderSourceShopName, sourceShopName, StringComparison.OrdinalIgnoreCase));
        foreach (var dependent in dependents)
        {
            dependent.SharedOrderSourceShopName = string.Empty;
            dependent.FirstOrder = [.. dependent.FirstOrder];
            dependent.LastOrder = [.. dependent.LastOrder];
        }
    }

    public static void ClearChildParentLinks(Category category, string parentShopName)
    {
        var children = category.Shops.Where(shop => string.Equals(shop.ParentShopName, parentShopName, StringComparison.OrdinalIgnoreCase));
        foreach (var child in children)
            child.ParentShopName = string.Empty;
    }

    public static bool IsTargetParentValid(Category targetCategory, string normalizedTargetParent, IReadOnlySet<string> subtreeNames)
    {
        if (string.IsNullOrWhiteSpace(normalizedTargetParent))
            return true;

        var parent = targetCategory.Shops.FirstOrDefault(shop => string.Equals(shop.Name, normalizedTargetParent, StringComparison.OrdinalIgnoreCase));
        return parent is not null && !subtreeNames.Contains(parent.Name);
    }
}