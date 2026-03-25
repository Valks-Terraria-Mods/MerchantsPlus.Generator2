namespace MerchantsPlus.Generator;

public sealed class MerchantShopBadgeRow{
    public MerchantShopBadgeRow(string shopKey, string categoryName, string shopName, string displayLabel, string backgroundHex, string foregroundHex, int itemCount)
    {
        ShopKey = shopKey;
        CategoryName = categoryName;
        ShopName = shopName;
        DisplayLabel = displayLabel;
        BackgroundHex = backgroundHex;
        ForegroundHex = foregroundHex;
        ItemCount = itemCount;
        ItemCountBadgeText = itemCount == 1 ? "1 Item" : $"{itemCount} Items";
    }

    public string ShopKey { get; }
    public string CategoryName { get; }
    public string ShopName { get; }
    public string DisplayLabel { get; }
    public string BackgroundHex { get; }
    public string ForegroundHex { get; }
    public int ItemCount { get; }
    public string ItemCountBadgeText { get; }
}
