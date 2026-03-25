namespace MerchantsPlus.Generator;

public sealed class UnorganizedCache{
    public string TModLoaderDllPath { get; set; } = string.Empty;
    public long TModLoaderLastWriteTimeUtcTicks { get; set; }
    public long TModLoaderLength { get; set; }
    public string[] ItemIds { get; set; } = [];
    public string[] UnorganizedItems { get; set; } = [];
}
