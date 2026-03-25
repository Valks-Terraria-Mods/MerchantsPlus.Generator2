namespace MerchantsPlus.Generator;

public class CatalogStorageGateway{
    public UnorganizedCache LoadUnorganizedCache() => CatalogStorage.LoadUnorganizedCache();

    public void SaveUnorganizedCache(UnorganizedCache cache) => CatalogStorage.SaveUnorganizedCache(cache);

    public GlobalFlagsConfig LoadGlobalFlagsConfig() => CatalogStorage.LoadGlobalFlagsConfig();

    public void SaveGlobalFlagsConfig(GlobalFlagsConfig config) => CatalogStorage.SaveGlobalFlagsConfig(config);

    public UnorganizedBlacklistConfig LoadUnorganizedBlacklistConfig() => CatalogStorage.LoadUnorganizedBlacklistConfig();

    public void SaveUnorganizedBlacklistConfig(UnorganizedBlacklistConfig config) => CatalogStorage.SaveUnorganizedBlacklistConfig(config);

    public void SaveCategoryConfigs(IReadOnlyList<CategoryConfig> categoryConfigs) => CatalogStorage.SaveCategoryConfigs(categoryConfigs);
}

public class ItemIdSource{
    public string[] GetAllItemIds() => [.. ItemIds.GetAll()];
}

public readonly struct TModLoaderFileSignature
{
    public TModLoaderFileSignature(string path, long lastWriteTimeUtcTicks, long length)
    {
        Path = path;
        LastWriteTimeUtcTicks = lastWriteTimeUtcTicks;
        Length = length;
    }

    public string Path { get; }
    public long LastWriteTimeUtcTicks { get; }
    public long Length { get; }
}

public class TModLoaderSignatureProvider{
    public TModLoaderFileSignature? TryGetSignature()
    {
        var tmodLoaderPath = TModLoaderLocator.ResolveTModLoaderDllPath();
        if (string.IsNullOrWhiteSpace(tmodLoaderPath) || !File.Exists(tmodLoaderPath))
            return null;

        var info = new FileInfo(tmodLoaderPath);
        return new TModLoaderFileSignature(Path.GetFullPath(tmodLoaderPath), info.LastWriteTimeUtc.Ticks, info.Length);
    }
}