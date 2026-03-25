namespace MerchantsPlus.Generator;

public sealed class MerchantWorkspaceComponent{
    private readonly MerchantBadgeColorComponent _badgeColorComponent;

    public MerchantWorkspaceComponent() : this(new MerchantBadgeColorComponent()) { }

    public MerchantWorkspaceComponent(MerchantBadgeColorComponent badgeColorComponent)
    {
        _badgeColorComponent = badgeColorComponent;
    }

    public MerchantWorkspaceBuildResult BuildWorkspaceNodes(
        IEnumerable<string> merchantNpcIds,
        string merchantFilterText,
        string merchantSortMode,
        Func<string, string, bool> matchesMerchantFilter,
        Func<string, MerchantAssignment> resolveEffectiveAssignment,
        Func<string, (bool Success, Category Category, Shop Shop)> resolveAssignableShop,
        Func<string, string, string> buildShopKey,
        string? preferredMerchantName)
    {
        var merchants = merchantNpcIds
            .Where(name => string.IsNullOrWhiteSpace(merchantFilterText) || matchesMerchantFilter(name, merchantFilterText))
            .Select(name =>
            {
                var assignment = resolveEffectiveAssignment(name);
                var badges = BuildShopBadges(assignment.Shops, false, resolveAssignableShop, buildShopKey);
                var itemCount = BuildItemCount(assignment.Shops, resolveAssignableShop);
                return (Name: name, Badges: badges, ShopCount: badges.Count, ItemCount: itemCount);
            });

        merchants = merchantSortMode switch
        {
            "Item Count" => merchants.OrderByDescending(entry => entry.ItemCount).ThenBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase),
            "Shop Count" => merchants.OrderByDescending(entry => entry.ShopCount).ThenBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase),
            _ => merchants.OrderBy(entry => entry.Name, StringComparer.OrdinalIgnoreCase)
        };

        var nodes = new List<MerchantTreeNode>();
        foreach (var entry in merchants)
        {
            var node = new MerchantTreeNode(entry.Name);
            foreach (var badge in entry.Badges)
                node.ShopBadges.Add(badge);

            node.ItemCountBadgeText = BuildItemCountBadgeText(entry.ItemCount);
            node.ShopCountBadgeText = BuildShopCountBadgeText(entry.ShopCount);
            nodes.Add(node);
        }

        if (nodes.Count == 0)
            return new MerchantWorkspaceBuildResult(nodes, string.Empty);

        var selected = string.IsNullOrWhiteSpace(preferredMerchantName)
            ? nodes[0].Title
            : (nodes.FirstOrDefault(node => string.Equals(node.Title, preferredMerchantName, StringComparison.OrdinalIgnoreCase))?.Title ?? nodes[0].Title);

        return new MerchantWorkspaceBuildResult(nodes, selected);
    }

    public MerchantWorkspaceEditorBuildResult BuildWorkspaceEditor(
        string selectedMerchantName,
        IEnumerable<string> ownedShopKeys,
        IReadOnlyDictionary<string, List<string>> shopOwners,
        IEnumerable<(Category Category, Shop Shop)> assignableShops,
        Func<string, string, string> buildShopKey,
        Func<string, (bool Success, Category Category, Shop Shop)> resolveAssignableShop)
    {
        if (string.IsNullOrWhiteSpace(selectedMerchantName))
            return new MerchantWorkspaceEditorBuildResult([], [], null);

        var ownedSet = ownedShopKeys.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var ownedBadges = BuildShopBadges(ownedShopKeys, false, resolveAssignableShop, buildShopKey);
        var availableBadges = new List<MerchantShopBadgeRow>();

        foreach (var (category, shop) in assignableShops.OrderBy(value => value.Category.Name, StringComparer.OrdinalIgnoreCase).ThenBy(value => value.Shop.Name, StringComparer.OrdinalIgnoreCase))
        {
            var shopKey = buildShopKey(category.Name, shop.Name);
            if (ownedSet.Contains(shopKey) || shopOwners.ContainsKey(shopKey))
                continue;

            var colors = _badgeColorComponent.BuildBadgeColors(shop.Name);
            var itemCount = CountShopItems(category, shop, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
            availableBadges.Add(new MerchantShopBadgeRow(
                shopKey,
                category.Name,
                shop.Name,
                $"{shop.Name} ({category.Name})",
                colors.BackgroundHex,
                colors.ForegroundHex,
                itemCount));
        }

        return new MerchantWorkspaceEditorBuildResult(ownedBadges, availableBadges, availableBadges.FirstOrDefault());
    }

    private IReadOnlyList<MerchantShopBadgeRow> BuildShopBadges(
        IEnumerable<string> shopKeys,
        bool includeCategoryInLabel,
        Func<string, (bool Success, Category Category, Shop Shop)> resolveAssignableShop,
        Func<string, string, string> buildShopKey)
    {
        return shopKeys
            .OrderBy(key => key, StringComparer.OrdinalIgnoreCase)
            .Select(shopKey =>
            {
                var resolved = resolveAssignableShop(shopKey);
                if (!resolved.Success)
                    return null;

                var colors = _badgeColorComponent.BuildBadgeColors(resolved.Shop.Name);
                var label = includeCategoryInLabel ? $"{resolved.Shop.Name} ({resolved.Category.Name})" : resolved.Shop.Name;
                var itemCount = CountShopItems(resolved.Category, resolved.Shop, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
                return new MerchantShopBadgeRow(
                    buildShopKey(resolved.Category.Name, resolved.Shop.Name),
                    resolved.Category.Name,
                    resolved.Shop.Name,
                    label,
                    colors.BackgroundHex,
                    colors.ForegroundHex,
                    itemCount);
            })
            .Where(value => value is not null)
            .Select(value => value!)
            .ToArray();
    }

    private static int BuildItemCount(IEnumerable<string> shopKeys, Func<string, (bool Success, Category Category, Shop Shop)> resolveAssignableShop)
    {
        var total = 0;
        foreach (var shopKey in shopKeys)
        {
            var resolved = resolveAssignableShop(shopKey);
            if (!resolved.Success)
                continue;

            total += CountShopItems(resolved.Category, resolved.Shop, new HashSet<string>(StringComparer.OrdinalIgnoreCase));
        }

        return total;
    }

    private static int CountShopItems(Category category, Shop shop, HashSet<string> visited)
    {
        if (!visited.Add(shop.Name))
            return 0;

        var total = shop.Items.Count;
        foreach (var nestedShop in category.Shops.Where(candidate => string.Equals(candidate.ParentShopName, shop.Name, StringComparison.OrdinalIgnoreCase)))
            total += CountShopItems(category, nestedShop, visited);

        return total;
    }

    private static string BuildShopCountBadgeText(int shopCount)
    {
        if (shopCount <= 0)
            return string.Empty;

        return shopCount == 1 ? "1 Shop" : $"{shopCount} Shops";
    }

    private static string BuildItemCountBadgeText(int itemCount)
    {
        if (itemCount <= 0)
            return string.Empty;

        return itemCount == 1 ? "1 Item" : $"{itemCount} Items";
    }

}
