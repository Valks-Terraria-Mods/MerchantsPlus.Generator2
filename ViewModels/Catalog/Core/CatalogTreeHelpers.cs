using Avalonia.Media;

namespace MerchantsPlus.Generator;

public abstract class CatalogTreeHelpers : CatalogActions
{
    protected CatalogItemRow CreateItemRow(string item, bool isShopSlot = false)
    {
        var category = GetSelectedCategory();
        var shop = GetSelectedShop();
        var global = ItemCatalog.GetGlobalFlagsConfig();
        var priceText = isShopSlot ? string.Empty : ItemPrices.FormatPrice(ItemPrices.GetPriceCopper(item));
        var isOverridden = !isShopSlot && ItemPrices.IsOverridden(item);
        var conditionCount = ItemConditions.GetConditions(item).Length;
        var sortFlag = string.Empty;

        var matchesFirst = MatchesOrder(item, global.FirstOrder, true)
            || (shop is not null && MatchesOrder(item, shop.FirstOrder, true))
            || (category is not null && MatchesOrder(item, category.FirstOrder, true));
        var matchesLast = MatchesOrder(item, global.LastOrder, false)
            || (shop is not null && MatchesOrder(item, shop.LastOrder, false))
            || (category is not null && MatchesOrder(item, category.LastOrder, false));

        if (matchesFirst)
            sortFlag = "First";
        else if (matchesLast)
            sortFlag = "Last";

        return new CatalogItemRow(item, Brushes.LightGray, priceText, isOverridden, sortFlag, conditionCount, isShopSlot);
    }

    protected Category? GetSelectedCategory()
    {
        if (string.IsNullOrWhiteSpace(SelectedCategoryForShop))
            return null;

        return ItemCatalog.Categories.FirstOrDefault(c => string.Equals(c.Name, SelectedCategoryForShop, StringComparison.OrdinalIgnoreCase));
    }

    protected Shop? GetSelectedShop()
    {
        if (_selectedNode?.Type is not (CatalogNodeType.Shop or CatalogNodeType.ShopSegment))
            return null;

        var category = GetSelectedCategory();
        return category?.Shops.FirstOrDefault(s => string.Equals(s.Name, SelectedShopName, StringComparison.OrdinalIgnoreCase));
    }

    protected void LoadSelectedCategoryInputs()
    {
        var category = GetSelectedCategory();
        if (category is null)
            return;

        NewCategoryName = category.Name;
        LoadRuleRows(CategoryKeywordRules, category.KeywordRules);
        NewCategoryRuleMode = KeywordRule.WhitelistMode;
        NewCategoryRuleMatch = KeywordRule.ContainsMatch;
        NewCategoryRuleTerm = string.Empty;
        NewCategoryFirstOrder = string.Join(", ", category.FirstOrder);
        NewCategoryLastOrder = string.Join(", ", category.LastOrder);
    }

    protected static bool ContainsNode(CatalogNode root, CatalogNode target)
    {
        if (ReferenceEquals(root, target))
            return true;

        foreach (var child in root.Children)
        {
            if (ContainsNode(child, target))
                return true;
        }

        return false;
    }

    protected static bool MatchesOrder(string item, string[] orderedTerms, bool startsWithFirst)
    {
        for (var i = 0; i < orderedTerms.Length; i++)
        {
            var term = orderedTerms[i];
            var isMatch = startsWithFirst && i == 0
                ? item.StartsWith(term, StringComparison.OrdinalIgnoreCase)
                : item.Contains(term, StringComparison.OrdinalIgnoreCase);

            if (isMatch)
                return true;
        }

        return false;
    }
}
