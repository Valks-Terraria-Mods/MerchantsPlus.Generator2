namespace MerchantsPlus.Generator;

public static class ItemCatalog
{
    private static readonly ItemCatalogService Service = CreateService();

    static ItemCatalog()
    {
        Service.Initialize();
    }

    public static IReadOnlyList<Category> Categories => Service.Categories;
    public static IReadOnlyList<string> Unorganized => Service.Unorganized;
    public static IReadOnlyList<string> BlacklistedUnorganized => Service.BlacklistedUnorganized;

    public static void BuildCatalog(bool forceRefresh = false) => Service.BuildCatalog(forceRefresh);

    public static void BuildCatalog(IEnumerable<string> items) => Service.BuildCatalog(items);

    public static GlobalFlagsConfig GetGlobalFlagsConfig() => Service.GetGlobalFlagsConfig();

    public static void UpdateGlobalFlagsConfig(IEnumerable<KeywordRule> rules, string[] firstOrder, string[] lastOrder)
        => Service.UpdateGlobalFlagsConfig(rules, firstOrder, lastOrder);

    public static UnorganizedBlacklistConfig GetUnorganizedBlacklistConfig() => Service.GetUnorganizedBlacklistConfig();

    public static void UpdateUnorganizedBlacklistConfig(IEnumerable<KeywordRule> rules)
        => Service.UpdateUnorganizedBlacklistConfig(rules);

    public static bool AddCategory(string categoryName, IEnumerable<KeywordRule>? rules = null, string[]? firstOrder = null, string[]? lastOrder = null)
        => Service.AddCategory(categoryName, rules, firstOrder, lastOrder);

    public static bool UpdateCategory(string existingName, string newName, IEnumerable<KeywordRule>? rules = null, string[]? firstOrder = null, string[]? lastOrder = null)
        => Service.UpdateCategory(existingName, newName, rules, firstOrder, lastOrder);

    public static bool RemoveCategory(string categoryName) => Service.RemoveCategory(categoryName);

    public static bool AddShop(string categoryName, Shop shop) => Service.AddShop(categoryName, shop);

    public static bool UpsertShop(string categoryName, string? existingShopName, Shop shop)
        => Service.UpsertShop(categoryName, existingShopName, shop);

    public static bool RemoveShop(string categoryName, string shopName) => Service.RemoveShop(categoryName, shopName);

    public static bool MoveShop(string sourceCategoryName, string shopName, string targetCategoryName, string? targetParentShopName)
        => Service.MoveShop(sourceCategoryName, shopName, targetCategoryName, targetParentShopName);

    public static void PrintCatalog() => Service.PrintCatalog();

    private static ItemCatalogService CreateService()
    {
        var state = new ItemCatalogState();
        CatalogStorageGateway storage = new CatalogStorageGateway();
        CatalogConfigurationSupport configurationSupport = new CatalogConfigurationSupport(state, storage);
        CatalogInitializationService initializationService = new CatalogInitializationService(state, storage, configurationSupport);
        ItemIdSource itemIdSource = new ItemIdSource();
        TModLoaderSignatureProvider signatureProvider = new TModLoaderSignatureProvider();
        CatalogBuildService buildService = new CatalogBuildService(state, storage, itemIdSource, signatureProvider);
        CatalogRulesService rulesService = new CatalogRulesService(state, storage);
        CatalogCategoryService categoryService = new CatalogCategoryService(state, configurationSupport);
        CatalogShopTreeService shopTreeService = new CatalogShopTreeService();
        CatalogShopService shopService = new CatalogShopService(state, configurationSupport, shopTreeService);
        return new ItemCatalogService(state, initializationService, buildService, rulesService, categoryService, shopService);
    }
}
