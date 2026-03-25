namespace MerchantsPlus.Generator;

public class ItemCatalogState{
    private readonly List<Category> _categories = [];
    private readonly List<string> _unorganized = [];
    private readonly List<string> _blacklistedUnorganized = [];

    private UnorganizedBlacklistConfig _unorganizedBlacklistConfig = new();
    private GlobalFlagsConfig _globalFlagsConfig = new();

    public IReadOnlyList<Category> Categories => _categories;
    public IReadOnlyList<string> Unorganized => _unorganized;
    public IReadOnlyList<string> BlacklistedUnorganized => _blacklistedUnorganized;

    public UnorganizedBlacklistConfig UnorganizedBlacklistConfig => _unorganizedBlacklistConfig;
    public GlobalFlagsConfig GlobalFlagsConfig => _globalFlagsConfig;

    internal List<Category> MutableCategories => _categories;
    internal List<string> MutableUnorganized => _unorganized;
    internal List<string> MutableBlacklistedUnorganized => _blacklistedUnorganized;

    public void SetUnorganizedBlacklistConfig(UnorganizedBlacklistConfig config)
    {
        _unorganizedBlacklistConfig = config;
    }

    public void SetGlobalFlagsConfig(GlobalFlagsConfig config)
    {
        _globalFlagsConfig = config;
    }
}