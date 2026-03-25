namespace MerchantsPlus.Generator;

public sealed class ShopConfig{
    public string Name { get; set; } = string.Empty;
    public int Priority { get; set; }
    public string[] Keywords { get; set; } = [];
    public KeywordRule[] KeywordRules { get; set; } = [];
    public string[] FirstOrder { get; set; } = [];
    public string[] LastOrder { get; set; } = [];
    public string[] ExcludedItems { get; set; } = [];
    public string SharedOrderSourceShopName { get; set; } = string.Empty;
    public string ParentShopName { get; set; } = string.Empty;
}
