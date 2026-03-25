namespace MerchantsPlus.Generator;

public sealed class CategoryConfig{
    public string Name { get; set; } = string.Empty;
    public string[] Keywords { get; set; } = [];
    public string[] BlacklistedKeywords { get; set; } = [];
    public KeywordRule[] KeywordRules { get; set; } = [];
    public string[] FirstOrder { get; set; } = [];
    public string[] LastOrder { get; set; } = [];
    public List<ShopConfig> Shops { get; set; } = [];
}
