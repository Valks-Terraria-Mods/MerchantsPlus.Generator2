namespace MerchantsPlus.Generator;

public sealed class CatalogCategoryActionsComponent{
    public bool TryAddCategory(
        string newCategoryName,
        KeywordRule[] rules,
        string[] firstOrder,
        string[] lastOrder,
        out string status)
    {
        var added = ItemCatalog.AddCategory(newCategoryName, rules, firstOrder, lastOrder);
        status = added
            ? "Category added."
            : "Category was not added (empty or duplicate name).";
        return added;
    }

    public bool TryUpdateCategory(
        string selectedCategoryForShop,
        string newCategoryName,
        KeywordRule[] rules,
        string[] firstOrder,
        string[] lastOrder,
        out string resolvedCategoryName,
        out string status)
    {
        resolvedCategoryName = string.Empty;
        if (string.IsNullOrWhiteSpace(selectedCategoryForShop) || string.IsNullOrWhiteSpace(newCategoryName))
        {
            status = "Select a category and enter a new category name.";
            return false;
        }

        var updated = ItemCatalog.UpdateCategory(selectedCategoryForShop, newCategoryName, rules, firstOrder, lastOrder);
        if (!updated)
        {
            status = "Category update failed (duplicate name or invalid input).";
            return false;
        }

        resolvedCategoryName = newCategoryName.Trim();
        status = "Category updated.";
        return true;
    }

    public bool TryRemoveCategory(string selectedCategoryForShop, out string status)
    {
        if (string.IsNullOrWhiteSpace(selectedCategoryForShop))
        {
            status = "Select a category to remove.";
            return false;
        }

        var removed = ItemCatalog.RemoveCategory(selectedCategoryForShop);
        status = removed ? "Category removed." : "Category removal failed.";
        return removed;
    }
}
