namespace MerchantsPlus.Generator;

public sealed class UnorganizedBlacklistConfig{
    public KeywordRule[] Rules { get; set; } = [];
    public string[] WhitelistKeywords { get; set; } = [];
    public string[] WhitelistPrefixKeywords { get; set; } = [];
    public string[] WhitelistSuffixKeywords { get; set; } = [];
    public string[] PrefixKeywords { get; set; } = [];
    public string[] SuffixKeywords { get; set; } = [];
    public string[] ContainsKeywords { get; set; } = [];
}
