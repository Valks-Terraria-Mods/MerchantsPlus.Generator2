namespace MerchantsPlus.Generator;

public sealed class CatalogMetricsComponent{
    private int _categoryCount;
    private int _shopCount;
    private int _itemCount;

    public int CategoryCount { get => _categoryCount; set => _categoryCount = value; }
    public int ShopCount { get => _shopCount; set => _shopCount = value; }
    public int ItemCount { get => _itemCount; set => _itemCount = value; }
}
