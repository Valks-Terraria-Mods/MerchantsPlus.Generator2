namespace MerchantsPlus.Generator;

public sealed class MerchantAssignmentActionsComponent{
    public bool SaveSelectedAssignments(
        string selectedMerchantName,
        HashSet<string> selectedCategories,
        HashSet<string> selectedShops,
        Func<string, HashSet<string>, HashSet<string>, List<string>> findConflicts,
        Func<string, MerchantAssignment> getOrCreateAssignment,
        out string assignmentHint,
        out string editorStatus)
    {
        assignmentHint = string.Empty;
        editorStatus = string.Empty;
        if (string.IsNullOrWhiteSpace(selectedMerchantName))
            return false;

        var conflicts = findConflicts(selectedMerchantName, selectedCategories, selectedShops);
        if (conflicts.Count > 0)
        {
            assignmentHint = "Assignment blocked by exclusivity rules. Resolve the conflicts listed below.";
            editorStatus = $"Could not save {selectedMerchantName}: {string.Join(" | ", conflicts)}";
            return false;
        }

        var assignment = getOrCreateAssignment(selectedMerchantName);
        assignment.Categories.Clear();
        assignment.Shops.Clear();
        foreach (var category in selectedCategories)
            assignment.Categories.Add(category);
        foreach (var shop in selectedShops)
            assignment.Shops.Add(shop);

        assignmentHint = "Saved. Category checkboxes now support partial selection (dash state).";
        editorStatus = $"Saved merchant assignments for {selectedMerchantName}.";
        return true;
    }

    public bool RemoveMerchantNode(
        string selectedMerchantName,
        MerchantTreeNode node,
        Func<string, MerchantAssignment> getOrCreateAssignment,
        Func<string, IEnumerable<string>> getShopKeysForCategory,
        Func<string, string, string> buildShopKey,
        out string editorStatus)
    {
        editorStatus = string.Empty;
        if (string.IsNullOrWhiteSpace(selectedMerchantName) || string.IsNullOrWhiteSpace(node.CategoryName))
            return false;

        var assignment = getOrCreateAssignment(selectedMerchantName);
        if (string.IsNullOrWhiteSpace(node.ShopName))
        {
            assignment.Categories.Remove(node.CategoryName);
            foreach (var shopKey in getShopKeysForCategory(node.CategoryName))
                assignment.Shops.Remove(shopKey);
            editorStatus = $"Removed category {node.CategoryName} from {selectedMerchantName}.";
            return true;
        }

        assignment.Shops.Remove(buildShopKey(node.CategoryName, node.ShopName));
        editorStatus = $"Removed shop {node.ShopName} from {selectedMerchantName}.";
        return true;
    }

    public bool AddSelectedMerchantShop(
        string selectedMerchantName,
        MerchantShopBadgeRow? selectedMerchantAddShop,
        Func<string, MerchantAssignment> getOrCreateAssignment,
        out string editorStatus)
    {
        editorStatus = string.Empty;
        if (string.IsNullOrWhiteSpace(selectedMerchantName) || selectedMerchantAddShop is null)
            return false;

        if (!getOrCreateAssignment(selectedMerchantName).Shops.Add(selectedMerchantAddShop.ShopKey))
            return false;

        editorStatus = $"Assigned {selectedMerchantAddShop.ShopName} to {selectedMerchantName}.";
        return true;
    }

    public bool RemoveMerchantOwnedShop(
        string selectedMerchantName,
        MerchantShopBadgeRow row,
        Func<string, MerchantAssignment> getOrCreateAssignment,
        out string editorStatus)
    {
        editorStatus = string.Empty;
        if (string.IsNullOrWhiteSpace(selectedMerchantName) || !getOrCreateAssignment(selectedMerchantName).Shops.Remove(row.ShopKey))
            return false;

        editorStatus = $"Removed {row.ShopName} from {selectedMerchantName}.";
        return true;
    }
}
