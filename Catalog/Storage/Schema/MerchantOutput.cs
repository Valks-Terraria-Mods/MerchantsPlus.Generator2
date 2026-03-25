namespace MerchantsPlus.Generator;

public sealed class MerchantOutput{
    public string Name { get; set; } = string.Empty;
    public List<ShopOutput> Shops { get; set; } = [];
}
