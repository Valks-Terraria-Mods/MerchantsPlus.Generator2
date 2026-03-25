using System.Collections.ObjectModel;

namespace MerchantsPlus.Generator;

public sealed class CatalogShopActionsComponent{
    public bool TrySaveShop(
        string selectedCategoryForShop,
        string selectedShopName,
        string newShopName,
        string newShopPriority,
        KeywordRule[] rules,
        string[] firstOrder,
        string[] lastOrder,
        out string savedShopName,
        out string status)
    {
        savedShopName = string.Empty;
        if (string.IsNullOrWhiteSpace(selectedCategoryForShop) || string.IsNullOrWhiteSpace(newShopName))
        {
            status = "Select a category and enter a shop name.";
            return false;
        }

        var shop = new Shop
        {
            Name = newShopName.Trim(),
            Priority = ParseShopPriority(newShopPriority),
            Keywords = KeywordRuleSet.ToLegacyContains(rules, KeywordRule.WhitelistMode),
            KeywordRules = rules,
            FirstOrder = firstOrder,
            LastOrder = lastOrder,
            ExcludedItems = KeywordRuleSet.ToLegacyContains(rules, KeywordRule.BlacklistMode)
        };

        var saved = ItemCatalog.UpsertShop(selectedCategoryForShop, selectedShopName, shop);
        if (!saved)
        {
            status = "Shop save failed (missing category or duplicate shop name).";
            return false;
        }

        savedShopName = shop.Name;
        status = "Shop saved.";
        return true;
    }

    public bool TryRemoveShop(string selectedCategoryForShop, string selectedShopName, out string status)
    {
        if (string.IsNullOrWhiteSpace(selectedCategoryForShop) || string.IsNullOrWhiteSpace(selectedShopName))
        {
            status = "Select a category and a shop to remove.";
            return false;
        }

        var removed = ItemCatalog.RemoveShop(selectedCategoryForShop, selectedShopName);
        status = removed ? "Shop removed." : "Shop removal failed.";
        return removed;
    }

    public bool TryMoveShop(string sourceCategoryName, string shopName, string targetCategoryName, string? targetParentShopName, out string status)
    {
        if (string.IsNullOrWhiteSpace(sourceCategoryName) || string.IsNullOrWhiteSpace(shopName))
        {
            status = "Select a shop to move first.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(targetCategoryName))
        {
            status = "Select a destination category.";
            return false;
        }

        if (!ItemCatalog.MoveShop(sourceCategoryName, shopName, targetCategoryName, targetParentShopName))
        {
            status = "Shop move failed (invalid destination, duplicate shop names, or a cyclic parent relationship).";
            return false;
        }

        var parentText = string.IsNullOrWhiteSpace(targetParentShopName)
            ? $"category '{targetCategoryName.Trim()}'"
            : $"shop '{targetParentShopName}'";
        status = $"Moved shop '{shopName}' to {parentText}.";
        return true;
    }

    public bool TryAddExcludedItemRule(
        string selectedCategoryForShop,
        string selectedShopName,
        string itemName,
        ObservableCollection<KeywordRuleEditorRow> shopKeywordRules,
        out string status)
    {
        if (string.IsNullOrWhiteSpace(selectedCategoryForShop) || string.IsNullOrWhiteSpace(selectedShopName))
        {
            status = "Select a shop first, then add a blacklisted keyword.";
            return false;
        }

        shopKeywordRules.Add(new KeywordRuleEditorRow
        {
            Mode = KeywordRule.BlacklistMode,
            Match = KeywordRule.ContainsMatch,
            Term = itemName
        });

        status = $"Added blacklisted keyword '{itemName}' to {selectedShopName}.";
        return true;
    }

    private static int ParseShopPriority(string value)
    {
        return int.TryParse(value, out var priority) ? priority : 0;
    }
}
