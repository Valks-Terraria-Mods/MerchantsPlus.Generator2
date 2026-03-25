namespace MerchantsPlus.Generator;

public class CatalogBuildService{
    private readonly ItemCatalogState _state;
    private readonly CatalogStorageGateway _storage;
    private readonly ItemIdSource _itemIdSource;
    private readonly TModLoaderSignatureProvider _signatureProvider;

    public CatalogBuildService(
        ItemCatalogState state,
        CatalogStorageGateway storage,
        ItemIdSource itemIdSource,
        TModLoaderSignatureProvider signatureProvider)
    {
        _state = state;
        _storage = storage;
        _itemIdSource = itemIdSource;
        _signatureProvider = signatureProvider;
    }

    public void BuildCatalog(bool forceRefresh = false)
    {
        var cache = _storage.LoadUnorganizedCache();
        var signature = _signatureProvider.TryGetSignature();
        var hasCacheItems = cache.ItemIds.Length > 0;
        var cacheMatches = signature is not null && CacheMatchesSignature(cache, signature.Value);
        var canUseCache = !forceRefresh && hasCacheItems && (cacheMatches || signature is null);

        var items = canUseCache
            ? cache.ItemIds
            : _itemIdSource.GetAllItemIds();

        BuildCatalog(items);

        if (!canUseCache && signature is not null)
        {
            _storage.SaveUnorganizedCache(new UnorganizedCache
            {
                TModLoaderDllPath = signature.Value.Path,
                TModLoaderLastWriteTimeUtcTicks = signature.Value.LastWriteTimeUtcTicks,
                TModLoaderLength = signature.Value.Length,
                ItemIds = items,
                UnorganizedItems = [.. _state.Unorganized]
            });
        }
    }

    public void BuildCatalog(IEnumerable<string> items)
    {
        ItemCatalogBuilder.Build(
            _state.MutableCategories,
            _state.MutableUnorganized,
            _state.MutableBlacklistedUnorganized,
            items,
            _state.UnorganizedBlacklistConfig,
            _state.GlobalFlagsConfig);
    }

    private static bool CacheMatchesSignature(UnorganizedCache cache, TModLoaderFileSignature signature)
    {
        if (cache.ItemIds.Length == 0)
            return false;

        return string.Equals(cache.TModLoaderDllPath, signature.Path, StringComparison.OrdinalIgnoreCase)
            && cache.TModLoaderLastWriteTimeUtcTicks == signature.LastWriteTimeUtcTicks
            && cache.TModLoaderLength == signature.Length;
    }
}