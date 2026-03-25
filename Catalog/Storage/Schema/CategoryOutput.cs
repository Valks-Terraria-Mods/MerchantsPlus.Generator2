namespace MerchantsPlus.Generator;

public sealed class CategoryOutput{
    public string Name { get; set; } = string.Empty;
    public List<ShopOutput> Shops { get; set; } = [];
}
