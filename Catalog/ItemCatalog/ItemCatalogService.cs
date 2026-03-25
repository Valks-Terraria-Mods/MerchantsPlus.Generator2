namespace MerchantsPlus.Generator;

public class ItemCatalogService{
    private readonly ItemCatalogState _state;
    private readonly CatalogInitializationService _initializationService;
    private readonly CatalogBuildService _buildService;
    private readonly CatalogRulesService _rulesService;
    private readonly CatalogCategoryService _categoryService;
    private readonly CatalogShopService _shopService;

    public ItemCatalogService(
        ItemCatalogState state,
        CatalogInitializationService initializationService,
        CatalogBuildService buildService,
        CatalogRulesService rulesService,
        CatalogCategoryService categoryService,
        CatalogShopService shopService)
    {
        _state = state;
        _initializationService = initializationService;
        _buildService = buildService;
        _rulesService = rulesService;
        _categoryService = categoryService;
        _shopService = shopService;
    }

    public IReadOnlyList<Category> Categories => _state.Categories;
    public IReadOnlyList<string> Unorganized => _state.Unorganized;
    public IReadOnlyList<string> BlacklistedUnorganized => _state.BlacklistedUnorganized;

    public void Initialize() => _initializationService.Initialize();

    public void BuildCatalog(bool forceRefresh = false) => _buildService.BuildCatalog(forceRefresh);

    public void BuildCatalog(IEnumerable<string> items) => _buildService.BuildCatalog(items);

    public GlobalFlagsConfig GetGlobalFlagsConfig() => _rulesService.GetGlobalFlagsConfig();

    public void UpdateGlobalFlagsConfig(IEnumerable<KeywordRule> rules, string[] firstOrder, string[] lastOrder)
        => _rulesService.UpdateGlobalFlagsConfig(rules, firstOrder, lastOrder);

    public UnorganizedBlacklistConfig GetUnorganizedBlacklistConfig() => _rulesService.GetUnorganizedBlacklistConfig();

    public void UpdateUnorganizedBlacklistConfig(IEnumerable<KeywordRule> rules)
        => _rulesService.UpdateUnorganizedBlacklistConfig(rules);

    public bool AddCategory(string categoryName, IEnumerable<KeywordRule>? rules = null, string[]? firstOrder = null, string[]? lastOrder = null)
        => _categoryService.AddCategory(categoryName, rules, firstOrder, lastOrder);

    public bool UpdateCategory(string existingName, string newName, IEnumerable<KeywordRule>? rules = null, string[]? firstOrder = null, string[]? lastOrder = null)
        => _categoryService.UpdateCategory(existingName, newName, rules, firstOrder, lastOrder);

    public bool RemoveCategory(string categoryName) => _categoryService.RemoveCategory(categoryName);

    public bool AddShop(string categoryName, Shop shop) => _shopService.AddShop(categoryName, shop);

    public bool UpsertShop(string categoryName, string? existingShopName, Shop shop)
        => _shopService.UpsertShop(categoryName, existingShopName, shop);

    public bool RemoveShop(string categoryName, string shopName) => _shopService.RemoveShop(categoryName, shopName);

    public bool MoveShop(string sourceCategoryName, string shopName, string targetCategoryName, string? targetParentShopName)
        => _shopService.MoveShop(sourceCategoryName, shopName, targetCategoryName, targetParentShopName);

    public void PrintCatalog()
    {
        foreach (var category in _state.Categories)
            Console.WriteLine(category);
    }
}