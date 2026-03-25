namespace MerchantsPlus.Generator;

internal static class ItemCatalogConfiguration
{
    internal static void LoadOrCreateDefaultConfiguration(List<Category> categories)
    {
        var configs = CatalogStorage.LoadCategoryConfigs();

        if (configs.Count == 0)
        {
            configs =
            [
                new CategoryConfig
                {
                    Name = "Furniture",
                    Keywords = [],
                    BlacklistedKeywords = [],
                    FirstOrder = [],
                    LastOrder = [],
                    Shops =
                    [
                        new ShopConfig
                        {
                            Name = "Chairs",
                            Priority = 0,
                            Keywords = ["chair"],
                            FirstOrder = [],
                            LastOrder = []
                        },
                        new ShopConfig
                        {
                            Name = "Tables",
                            Priority = 0,
                            Keywords = ["table"],
                            FirstOrder = [],
                            LastOrder = []
                        }
                    ]
                }
            ];

            CatalogStorage.SaveCategoryConfigs(configs);
        }

        LoadFromConfigs(categories, configs);
    }

    internal static List<CategoryConfig> ToConfigs(IEnumerable<Category> categories)
    {
        return
        [
            .. categories.Select(c => new CategoryConfig
            {
                Name = c.Name,
                Keywords = KeywordRuleSet.ToLegacyContains(c.KeywordRules, KeywordRule.WhitelistMode),
                BlacklistedKeywords = KeywordRuleSet.ToLegacyContains(c.KeywordRules, KeywordRule.BlacklistMode),
                KeywordRules = KeywordRuleSet.Normalize(c.KeywordRules),
                FirstOrder = c.FirstOrder,
                LastOrder = c.LastOrder,
                Shops =
                [
                    .. c.Shops.Select(s => new ShopConfig
                    {
                        Name = s.Name,
                        Priority = s.Priority,
                        Keywords = KeywordRuleSet.ToLegacyContains(s.KeywordRules, KeywordRule.WhitelistMode),
                        KeywordRules = KeywordRuleSet.Normalize(s.KeywordRules),
                        FirstOrder = s.FirstOrder,
                        LastOrder = s.LastOrder,
                        ExcludedItems = KeywordRuleSet.ToLegacyContains(s.KeywordRules, KeywordRule.BlacklistMode),
                        SharedOrderSourceShopName = s.SharedOrderSourceShopName,
                        ParentShopName = s.ParentShopName
                    })
                ]
            })
        ];
    }

    internal static void ApplyCategoryAndSharedOrderLinks(Category category)
    {
        var shopMap = category.Shops.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var shop in category.Shops)
        {
            shop.CategoryKeywords = KeywordRuleSet.ToLegacyContains(category.KeywordRules, KeywordRule.WhitelistMode);
            shop.CategoryBlacklistedKeywords = KeywordRuleSet.ToLegacyContains(category.KeywordRules, KeywordRule.BlacklistMode);
            shop.CategoryKeywordRules = category.KeywordRules;
            shop.CategoryFirstOrder = category.FirstOrder;
            shop.CategoryLastOrder = category.LastOrder;
        }

        foreach (var shop in category.Shops)
        {
            if (string.IsNullOrWhiteSpace(shop.SharedOrderSourceShopName))
                continue;

            if (!shopMap.TryGetValue(shop.SharedOrderSourceShopName, out var sourceShop) || ReferenceEquals(sourceShop, shop))
            {
                shop.SharedOrderSourceShopName = string.Empty;
                continue;
            }

            shop.FirstOrder = sourceShop.FirstOrder;
            shop.LastOrder = sourceShop.LastOrder;
        }

        NormalizeParentShopLinks(category);
    }

    internal static void UpdateDependentSourceNames(Category category, string oldName, string newName)
    {
        foreach (var shop in category.Shops)
        {
            if (string.Equals(shop.SharedOrderSourceShopName, oldName, StringComparison.OrdinalIgnoreCase))
                shop.SharedOrderSourceShopName = newName;

            if (string.Equals(shop.ParentShopName, oldName, StringComparison.OrdinalIgnoreCase))
                shop.ParentShopName = newName;
        }
    }

    private static void LoadFromConfigs(List<Category> categories, IEnumerable<CategoryConfig> configs)
    {
        categories.Clear();

        foreach (var config in configs)
        {
            var normalizedCategoryRules = KeywordRuleSet.Normalize(config.KeywordRules ?? []);
            if (normalizedCategoryRules.Length == 0 && (config.Keywords.Length > 0 || config.BlacklistedKeywords.Length > 0))
            {
                normalizedCategoryRules = KeywordRuleSet.Normalize(KeywordRuleSet.FromLegacyCategory(config.Keywords, config.BlacklistedKeywords));
            }

            categories.Add(new Category
            {
                Name = config.Name,
                KeywordRules = normalizedCategoryRules,
                FirstOrder = config.FirstOrder,
                LastOrder = config.LastOrder,
                Shops =
                [
                    .. config.Shops.Select(s =>
                    {
                        var normalizedShopRules = KeywordRuleSet.Normalize(s.KeywordRules ?? []);
                        if (normalizedShopRules.Length == 0 && (s.Keywords.Length > 0 || s.ExcludedItems.Length > 0))
                        {
                            normalizedShopRules = KeywordRuleSet.Normalize(KeywordRuleSet.FromLegacyShop(s.Keywords, s.ExcludedItems));
                        }

                        return new Shop
                        {
                            Name = s.Name,
                            Priority = s.Priority,
                            Keywords = [],
                            KeywordRules = normalizedShopRules,
                            FirstOrder = s.FirstOrder,
                            LastOrder = s.LastOrder,
                            SharedOrderSourceShopName = s.SharedOrderSourceShopName,
                            ParentShopName = s.ParentShopName
                        };
                    })
                ]
            });

            categories[^1].Keywords = KeywordRuleSet.ToLegacyContains(categories[^1].KeywordRules, KeywordRule.WhitelistMode);
            categories[^1].BlacklistedKeywords = KeywordRuleSet.ToLegacyContains(categories[^1].KeywordRules, KeywordRule.BlacklistMode);

            foreach (var shop in categories[^1].Shops)
            {
                shop.Keywords = KeywordRuleSet.ToLegacyContains(shop.KeywordRules, KeywordRule.WhitelistMode);
                shop.ExcludedItems = KeywordRuleSet.ToLegacyContains(shop.KeywordRules, KeywordRule.BlacklistMode);
            }

            ApplyCategoryAndSharedOrderLinks(categories[^1]);
        }
    }

    private static void NormalizeParentShopLinks(Category category)
    {
        var shopMap = category.Shops.ToDictionary(s => s.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var shop in category.Shops)
        {
            if (string.IsNullOrWhiteSpace(shop.ParentShopName))
                continue;

            if (!shopMap.ContainsKey(shop.ParentShopName) || string.Equals(shop.ParentShopName, shop.Name, StringComparison.OrdinalIgnoreCase))
            {
                shop.ParentShopName = string.Empty;
                continue;
            }

            var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { shop.Name };
            var parentName = shop.ParentShopName;
            while (!string.IsNullOrWhiteSpace(parentName))
            {
                if (!visited.Add(parentName))
                {
                    shop.ParentShopName = string.Empty;
                    break;
                }

                if (!shopMap.TryGetValue(parentName, out var parent))
                    break;

                parentName = parent.ParentShopName;
            }
        }
    }
}
