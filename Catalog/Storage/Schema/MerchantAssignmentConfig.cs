namespace MerchantsPlus.Generator;

public sealed class MerchantAssignmentConfig{
    public string MerchantName { get; set; } = string.Empty;
    public string[] Categories { get; set; } = [];
    public string[] Shops { get; set; } = [];
}
