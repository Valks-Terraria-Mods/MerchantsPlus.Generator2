namespace MerchantsPlus.Generator;

public sealed class ShopMoveDestination{
    public required string Label { get; init; }
    public required string CategoryName { get; init; }
    public string ParentShopName { get; init; } = string.Empty;

    public override string ToString() => Label;
}
