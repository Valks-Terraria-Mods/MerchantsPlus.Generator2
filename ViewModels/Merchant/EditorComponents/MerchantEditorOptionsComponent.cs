namespace MerchantsPlus.Generator;

public sealed class MerchantEditorOptionsComponent{
    public Dictionary<string, bool> CaptureExpandedState(
        IEnumerable<AssignmentTreeNode> optionsTree,
        IEnumerable<AssignmentTreeNode> availableTree,
        IEnumerable<AssignmentTreeNode> assignedTree)
    {
        return optionsTree
            .Concat(availableTree)
            .Concat(assignedTree)
            .DistinctBy(node => node.Option.Key)
            .ToDictionary(node => node.Option.Key, node => node.IsExpanded, StringComparer.OrdinalIgnoreCase);
    }

    public MerchantEditorOptionsBuildResult BuildOptions(
        string selectedMerchantName,
        IReadOnlyList<Category> categories,
        MerchantAssignment effectiveAssignment,
        IReadOnlyDictionary<string, List<string>> categoryOwners,
        IReadOnlyDictionary<string, List<string>> shopOwners,
        IReadOnlyDictionary<string, bool> expandedState,
        Func<string, string, string> buildShopKey,
        Func<string, bool> isAssignableShopKey,
        Func<string, string> buildCategoryMerchantSuffix,
        Action<SelectableOption, IEnumerable<SelectableOption>> updateCategoryState,
        Action<SelectableOption, List<SelectableOption>, SelectableOption> wireShopOption,
        Action<SelectableOption, List<SelectableOption>> wireCategoryOption)
    {
        var categoryOptions = new List<SelectableOption>();
        var shopOptions = new List<SelectableOption>();
        var optionsTree = new List<AssignmentTreeNode>();
        var availableTree = new List<AssignmentTreeNode>();
        var assignedTree = new List<AssignmentTreeNode>();
        var assignmentHint = "Assign categories and shops to the selected merchant.";

        if (string.IsNullOrWhiteSpace(selectedMerchantName))
            return new MerchantEditorOptionsBuildResult(categoryOptions, shopOptions, optionsTree, availableTree, assignedTree, assignmentHint);

        foreach (var category in categories.OrderBy(value => value.Name, StringComparer.OrdinalIgnoreCase))
        {
            var isSelectedCategory = effectiveAssignment.Categories.Contains(category.Name);
            var categoryLocked = categoryOwners.ContainsKey(category.Name) && !isSelectedCategory;
            var categoryOption = new SelectableOption(category.Name, category.Name, isSelectedCategory, !categoryLocked);

            categoryOptions.Add(categoryOption);
            var categoryNode = new AssignmentTreeNode(categoryOption, true)
            {
                IsExpanded = expandedState.TryGetValue(categoryOption.Key, out var expanded) && expanded
            };

            optionsTree.Add(categoryNode);
            var childOptions = new List<SelectableOption>();

            foreach (var shop in category.Shops.OrderBy(value => value.Name, StringComparer.OrdinalIgnoreCase))
            {
                var shopKey = buildShopKey(category.Name, shop.Name);
                if (!isAssignableShopKey(shopKey))
                    continue;

                var isSelectedShop = effectiveAssignment.Shops.Contains(shopKey);
                var ownerText = BuildOwnerText(shopOwners, shopKey, isSelectedShop);
                var shopOption = new SelectableOption(shopKey, $"{shop.Name}{ownerText}", isSelectedShop, string.IsNullOrEmpty(ownerText));

                shopOptions.Add(shopOption);
                categoryNode.Children.Add(new AssignmentTreeNode(shopOption, false));
                childOptions.Add(shopOption);
                wireShopOption(categoryOption, childOptions, shopOption);
            }

            updateCategoryState(categoryOption, childOptions);
            wireCategoryOption(categoryOption, childOptions);
        }

        BuildSplitTrees(optionsTree, availableTree, assignedTree, expandedState, buildCategoryMerchantSuffix);

        var lockedCount = categoryOptions.Count(option => !option.IsEnabled) + shopOptions.Count(option => !option.IsEnabled);
        if (lockedCount > 0)
            assignmentHint = "Some categories/shops are locked because they are already assigned to other merchants.";

        return new MerchantEditorOptionsBuildResult(categoryOptions, shopOptions, optionsTree, availableTree, assignedTree, assignmentHint);
    }

    private static string BuildOwnerText(IReadOnlyDictionary<string, List<string>> shopOwners, string shopKey, bool isSelectedShop)
    {
        return shopOwners.TryGetValue(shopKey, out var owners) && !isSelectedShop
            ? $" (assigned to {string.Join(", ", owners)})"
            : string.Empty;
    }

    private static void BuildSplitTrees(
        IEnumerable<AssignmentTreeNode> optionsTree,
        ICollection<AssignmentTreeNode> availableTree,
        ICollection<AssignmentTreeNode> assignedTree,
        IReadOnlyDictionary<string, bool> expandedState,
        Func<string, string> buildCategoryMerchantSuffix)
    {
        foreach (var categoryNode in optionsTree)
        {
            var availableCategoryNode = new AssignmentTreeNode(categoryNode.Option, true)
            {
                IsExpanded = expandedState.TryGetValue(categoryNode.Option.Key, out var availableExpanded) && availableExpanded
            };
            var assignedCategoryNode = new AssignmentTreeNode(
                categoryNode.Option,
                true,
                categoryNode.Option.Label,
                buildCategoryMerchantSuffix(categoryNode.Option.Key))
            {
                IsExpanded = expandedState.TryGetValue(categoryNode.Option.Key, out var assignedExpanded) && assignedExpanded
            };

            foreach (var shopNode in categoryNode.Children)
            {
                if (IsAvailable(shopNode.Option))
                    availableCategoryNode.Children.Add(new AssignmentTreeNode(shopNode.Option, false));

                if (IsAssigned(shopNode.Option))
                    assignedCategoryNode.Children.Add(new AssignmentTreeNode(shopNode.Option, false));
            }

            var includeAvailableCategory = IsAvailable(categoryNode.Option) || availableCategoryNode.Children.Count > 0;
            var includeAssignedCategory = IsAssigned(categoryNode.Option) || assignedCategoryNode.Children.Count > 0;

            if (includeAvailableCategory)
                availableTree.Add(availableCategoryNode);

            if (includeAssignedCategory)
                assignedTree.Add(assignedCategoryNode);
        }
    }

    private static bool IsAvailable(SelectableOption option)
    {
        return option.IsEnabled && option.IsSelected != true;
    }

    private static bool IsAssigned(SelectableOption option)
    {
        return option.IsSelected == true || !option.IsEnabled;
    }
}
