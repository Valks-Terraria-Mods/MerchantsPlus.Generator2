namespace MerchantsPlus.Generator;

internal static class ItemCatalogBuilder
{
    internal static void Build(
        List<Category> categories,
        List<string> unorganized,
        List<string> blacklistedUnorganized,
        IEnumerable<string> items,
        UnorganizedBlacklistConfig unorganizedBlacklist,
        GlobalFlagsConfig globalFlags)
    {
        ResetCatalog(categories, unorganized, blacklistedUnorganized);
        ApplyGlobalSortFlags(categories, globalFlags);
        var prioritizedShops = BuildPrioritizedShopList(categories);
        var globalRules = globalFlags.Rules.Length > 0
            ? KeywordRuleSet.Normalize(globalFlags.Rules)
            : KeywordRuleSet.FromLegacyUnorganized(new UnorganizedBlacklistConfig
            {
                WhitelistKeywords = globalFlags.WhitelistKeywords,
                WhitelistPrefixKeywords = globalFlags.WhitelistPrefixKeywords,
                WhitelistSuffixKeywords = globalFlags.WhitelistSuffixKeywords,
                PrefixKeywords = globalFlags.PrefixKeywords,
                SuffixKeywords = globalFlags.SuffixKeywords,
                ContainsKeywords = globalFlags.ContainsKeywords
            });

        foreach (var item in items)
        {
            if (IsUnorganizedBlacklisted(item, unorganizedBlacklist))
            {
                blacklistedUnorganized.Add(item);
                continue;
            }

            var owningShop = FindOwningShop(item, prioritizedShops, globalRules);

            if (owningShop is null)
                unorganized.Add(item);
            else
                owningShop.AddItem(item);
        }
    }

    private static List<Shop> BuildPrioritizedShopList(IReadOnlyList<Category> categories)
    {
        return
        [
            .. categories
                .SelectMany(category => category.Shops.Select((shop, shopIndex) => new { shop, categoryName = category.Name, shopIndex }))
                .OrderBy(v => v.shop.Priority)
                // Keep ownership ties stable across runtime edits and app restarts.
                .ThenBy(v => v.categoryName, StringComparer.OrdinalIgnoreCase)
                .ThenBy(v => v.shopIndex)
                .Select(v => v.shop)
        ];
    }

    private static Shop? FindOwningShop(string item, IReadOnlyList<Shop> prioritizedShops, IReadOnlyList<KeywordRule> globalRules)
    {
        var matchesGlobalWhitelist = KeywordRuleMatcher.MatchesAny(item, globalRules, KeywordRule.WhitelistMode);

        foreach (var shop in prioritizedShops)
        {
            if (IsCategoryFilteredOut(item, shop))
                continue;

            if (KeywordRuleMatcher.MatchesAny(item, globalRules, KeywordRule.BlacklistMode))
                continue;

            var shopRules = shop.KeywordRules.Length > 0
                ? shop.KeywordRules
                : KeywordRuleSet.FromLegacyShop(shop.Keywords, shop.ExcludedItems);

            if (KeywordRuleMatcher.MatchesAny(item, shopRules, KeywordRule.BlacklistMode))
                continue;

            if (matchesGlobalWhitelist || KeywordRuleMatcher.MatchesAny(item, shopRules, KeywordRule.WhitelistMode))
                return shop;
        }

        return null;
    }

    private static void ResetCatalog(List<Category> categories, List<string> unorganized, List<string> blacklistedUnorganized)
    {
        unorganized.Clear();
        blacklistedUnorganized.Clear();

        foreach (var category in categories)
        {
            foreach (var shop in category.Shops)
                shop.Clear();
        }
    }

    private static bool IsCategoryFilteredOut(string item, Shop shop)
    {
        var categoryRules = shop.CategoryKeywordRules.Length > 0
            ? shop.CategoryKeywordRules
            : KeywordRuleSet.FromLegacyCategory(shop.CategoryKeywords, shop.CategoryBlacklistedKeywords);

        if (KeywordRuleMatcher.MatchesAny(item, categoryRules, KeywordRule.BlacklistMode))
        {
            return true;
        }

        var hasWhitelist = categoryRules.Any(v => string.Equals(v.Mode, KeywordRule.WhitelistMode, StringComparison.OrdinalIgnoreCase));
        if (!hasWhitelist)
            return false;

        return !KeywordRuleMatcher.MatchesAny(item, categoryRules, KeywordRule.WhitelistMode);
    }

    private static bool IsUnorganizedBlacklisted(string item, UnorganizedBlacklistConfig config)
    {
        var rules = config.Rules.Length > 0 ? config.Rules : KeywordRuleSet.FromLegacyUnorganized(config);

        if (KeywordRuleMatcher.MatchesAny(item, rules, KeywordRule.WhitelistMode))
            return false;

        return KeywordRuleMatcher.MatchesAny(item, rules, KeywordRule.BlacklistMode);
    }

    private static void ApplyGlobalSortFlags(IEnumerable<Category> categories, GlobalFlagsConfig globalFlags)
    {
        var first = globalFlags.FirstOrder ?? [];
        var last = globalFlags.LastOrder ?? [];

        foreach (var shop in categories.SelectMany(category => category.Shops))
        {
            shop.GlobalFirstOrder = first;
            shop.GlobalLastOrder = last;
        }
    }
}
