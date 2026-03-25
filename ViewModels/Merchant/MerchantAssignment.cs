namespace MerchantsPlus.Generator;

public sealed class MerchantAssignment{
    public HashSet<string> Categories { get; } = new(StringComparer.OrdinalIgnoreCase);
    public HashSet<string> Shops { get; } = new(StringComparer.OrdinalIgnoreCase);
}
