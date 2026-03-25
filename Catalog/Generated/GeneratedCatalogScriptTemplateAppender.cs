using System.Text;

namespace MerchantsPlus.Generator;

internal static class GeneratedCatalogScriptTemplateAppender
{
    public static void AppendIterationMethods(StringBuilder sb)
    {
        sb.AppendLine("    public static IEnumerable<GeneratedMerchant> EnumerateMerchants() => Merchants;");
        sb.AppendLine();
        sb.AppendLine("    public static IEnumerable<GeneratedShopRef> EnumerateShops()");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var merchant in Merchants)");
        sb.AppendLine("        {");
        sb.AppendLine("            foreach (var shop in merchant.Shops)");
        sb.AppendLine("                yield return new GeneratedShopRef(merchant.Name, shop.Name, shop);");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static IEnumerable<GeneratedShopRef> EnumerateAllShops(bool includeNestedShops = true)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var topLevel in EnumerateShops())");
        sb.AppendLine("        {");
        sb.AppendLine("            yield return topLevel;");
        sb.AppendLine();
        sb.AppendLine("            if (!includeNestedShops)");
        sb.AppendLine("                continue;");
        sb.AppendLine();
        sb.AppendLine("            foreach (var nested in EnumerateNestedShops(topLevel.MerchantName, topLevel.Shop))");
        sb.AppendLine("                yield return nested;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static IEnumerable<GeneratedItemRef> EnumerateItems(bool includeNestedShops = true)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var topLevel in EnumerateShops())");
        sb.AppendLine("        {");
        sb.AppendLine("            foreach (var item in EnumerateShopItems(topLevel.MerchantName, topLevel.ShopName, topLevel.Shop, includeNestedShops))");
        sb.AppendLine("                yield return item;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static IEnumerable<GeneratedItemRef> EnumerateShopSlotItems(bool includeNestedShops = true)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var item in EnumerateItems(includeNestedShops))");
        sb.AppendLine("        {");
        sb.AppendLine("            if (item.Item.IsShopSlot)");
        sb.AppendLine("                yield return item;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static IEnumerable<GeneratedItemRef> EnumeratePricedItems(bool includeNestedShops = true)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var item in EnumerateItems(includeNestedShops))");
        sb.AppendLine("        {");
        sb.AppendLine("            if (!string.IsNullOrWhiteSpace(item.Item.Price))");
        sb.AppendLine("                yield return item;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static IEnumerable<GeneratedItemRef> EnumerateConditionedItems(bool includeNestedShops = true)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var item in EnumerateItems(includeNestedShops))");
        sb.AppendLine("        {");
        sb.AppendLine("            if (item.Item.Conditions.Count > 0)");
        sb.AppendLine("                yield return item;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    private static IEnumerable<GeneratedShopRef> EnumerateNestedShops(string merchantName, GeneratedShop shop)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var item in shop.Items)");
        sb.AppendLine("        {");
        sb.AppendLine("            if (item.NestedShop is null)");
        sb.AppendLine("                continue;");
        sb.AppendLine();
        sb.AppendLine("            yield return new GeneratedShopRef(merchantName, item.NestedShop.Name, item.NestedShop);");
        sb.AppendLine();
        sb.AppendLine("            foreach (var nested in EnumerateNestedShops(merchantName, item.NestedShop))");
        sb.AppendLine("                yield return nested;");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    private static IEnumerable<GeneratedItemRef> EnumerateShopItems(string merchantName, string rootShopName, GeneratedShop shop, bool includeNestedShops)");
        sb.AppendLine("    {");
        sb.AppendLine("        foreach (var item in shop.Items)");
        sb.AppendLine("        {");
        sb.AppendLine("            yield return new GeneratedItemRef(merchantName, rootShopName, shop.Name, item);");
        sb.AppendLine();
        sb.AppendLine("            if (includeNestedShops && item.NestedShop is not null)");
        sb.AppendLine("            {");
        sb.AppendLine("                foreach (var nestedItem in EnumerateShopItems(merchantName, rootShopName, item.NestedShop, includeNestedShops))");
        sb.AppendLine("                    yield return nestedItem;");
        sb.AppendLine("            }");
        sb.AppendLine("        }");
        sb.AppendLine("    }");
    }

    public static void AppendDataTypes(StringBuilder sb)
    {
        sb.AppendLine("public sealed record GeneratedMerchant(string Name, IReadOnlyList<GeneratedShop> Shops);");
        sb.AppendLine();
        sb.AppendLine("public sealed record GeneratedShop(string Name, IReadOnlyList<GeneratedItem> Items);");
        sb.AppendLine();
        sb.AppendLine("public sealed record GeneratedItem(string Name, string? Price, IReadOnlyList<string> Conditions, bool IsShopSlot, GeneratedShop? NestedShop);");
        sb.AppendLine();
        sb.AppendLine("public sealed record GeneratedShopRef(string MerchantName, string ShopName, GeneratedShop Shop);");
        sb.AppendLine();
        sb.AppendLine("public sealed record GeneratedItemRef(string MerchantName, string RootShopName, string ShopName, GeneratedItem Item)");
        sb.AppendLine("{");
        sb.AppendLine("    public string ItemName => Item.Name;");
        sb.AppendLine("    public string? Price => Item.Price;");
        sb.AppendLine("    public IReadOnlyList<string> Conditions => Item.Conditions;");
        sb.AppendLine("    public bool IsShopSlot => Item.IsShopSlot;");
        sb.AppendLine("    public GeneratedShop? NestedShop => Item.NestedShop;");
        sb.AppendLine("}");
    }
}
