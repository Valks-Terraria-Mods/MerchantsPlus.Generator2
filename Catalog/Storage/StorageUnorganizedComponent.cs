using System.Text.Json;

namespace MerchantsPlus.Generator;

internal static class StorageUnorganizedComponent
{
    internal static UnorganizedCache LoadUnorganizedCache(string unorganizedDirectory, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(unorganizedDirectory);
        var path = Path.Combine(unorganizedDirectory, "unorganized.json");

        if (!File.Exists(path))
            return new UnorganizedCache();

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<UnorganizedCache>(json, options) ?? new UnorganizedCache();
        data.ItemIds ??= [];
        data.UnorganizedItems ??= [];
        return data;
    }

    internal static void SaveUnorganizedCache(string unorganizedDirectory, UnorganizedCache cache, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(unorganizedDirectory);
        var path = Path.Combine(unorganizedDirectory, "unorganized.json");

        var stable = new UnorganizedCache
        {
            TModLoaderDllPath = cache.TModLoaderDllPath ?? string.Empty,
            TModLoaderLastWriteTimeUtcTicks = cache.TModLoaderLastWriteTimeUtcTicks,
            TModLoaderLength = cache.TModLoaderLength,
            ItemIds = cache.ItemIds ?? [],
            UnorganizedItems = cache.UnorganizedItems ?? []
        };

        var json = JsonSerializer.Serialize(stable, options);
        json = StoragePathHelper.ConvertToFourSpaceIndent(json);
        File.WriteAllText(path, json);
    }
}
