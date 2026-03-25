namespace MerchantsPlus.Generator;

public sealed class ShopOutput{
    public string Name { get; set; } = string.Empty;
    public List<ItemOutput> Items { get; set; } = [];
}
