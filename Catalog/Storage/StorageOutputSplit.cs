namespace MerchantsPlus.Generator;

internal static class StorageOutputSplit
{
    internal static IEnumerable<ShopOutput> SplitShopOutput(Category category, Shop shop)
    {
        return StorageShopOutputBuilder.BuildShopOutputs(category, shop);
    }
}
