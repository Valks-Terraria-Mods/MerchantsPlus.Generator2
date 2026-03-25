namespace MerchantsPlus.Generator;

internal static class StorageShopOutputBuilder
{
    private const int MaxItemsPerShop = 40;

    internal static IEnumerable<ShopOutput> BuildTopLevelShopOutputs(Category category, IEnumerable<Shop> selectedShops)
    {
        var selected = selectedShops.ToList();
        if (selected.Count == 0)
            return [];

        var selectedNames = selected.Select(shop => shop.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);

        return selected
            .Where(shop => string.IsNullOrWhiteSpace(shop.ParentShopName) || !selectedNames.Contains(shop.ParentShopName))
            .SelectMany(shop => BuildShopOutputs(category, shop));
    }

    internal static IEnumerable<ShopOutput> BuildShopOutputs(Category category, Shop shop)
    {
        var slots = BuildShopSlots(category, shop).ToList();

        if (slots.Count <= MaxItemsPerShop)
        {
            yield return new ShopOutput
            {
                Name = shop.Name,
                Items = slots
            };
            yield break;
        }

        var chunkIndex = 1;
        for (var i = 0; i < slots.Count; i += MaxItemsPerShop)
        {
            yield return new ShopOutput
            {
                Name = $"{shop.Name} {chunkIndex}",
                Items = slots.Skip(i).Take(MaxItemsPerShop).ToList()
            };

            chunkIndex++;
        }
    }

    private static IEnumerable<ItemOutput> BuildShopSlots(Category category, Shop shop)
    {
        foreach (var item in shop.Items)
        {
            yield return new ItemOutput
            {
                Name = item,
                Price = ItemPrices.FormatPrice(ItemPrices.GetPriceCopper(item)),
                Conditions = GetConditionsOrNull(item)
            };
        }

        foreach (var nestedShop in category.Shops.Where(candidate => string.Equals(candidate.ParentShopName, shop.Name, StringComparison.OrdinalIgnoreCase)))
        {
            yield return new ItemOutput
            {
                Name = nestedShop.Name,
                IsShopSlot = true,
                Shop = BuildNestedShopOutput(category, nestedShop, new HashSet<string>(StringComparer.OrdinalIgnoreCase))
            };
        }
    }

    private static ShopOutput BuildNestedShopOutput(Category category, Shop shop, HashSet<string> visitedShops)
    {
        if (!visitedShops.Add(shop.Name))
        {
            return new ShopOutput
            {
                Name = shop.Name,
                Items = []
            };
        }

        var slots = new List<ItemOutput>();

        foreach (var item in shop.Items)
        {
            slots.Add(new ItemOutput
            {
                Name = item,
                Price = ItemPrices.FormatPrice(ItemPrices.GetPriceCopper(item)),
                Conditions = GetConditionsOrNull(item)
            });
        }

        foreach (var nestedShop in category.Shops.Where(candidate => string.Equals(candidate.ParentShopName, shop.Name, StringComparison.OrdinalIgnoreCase)))
        {
            slots.Add(new ItemOutput
            {
                Name = nestedShop.Name,
                IsShopSlot = true,
                Shop = BuildNestedShopOutput(category, nestedShop, visitedShops)
            });
        }

        return new ShopOutput
        {
            Name = shop.Name,
            Items = slots
        };
    }

    private static string[]? GetConditionsOrNull(string item)
    {
        var conditions = ItemConditions.GetConditions(item);
        return conditions.Length == 0 ? null : conditions;
    }
}
