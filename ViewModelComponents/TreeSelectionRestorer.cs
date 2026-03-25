namespace MerchantsPlus.Generator;

internal static class TreeSelectionRestorer
{
    internal static void RestoreSelection(CatalogTree tree, string selectedCategory, string selectedShop)
    {
        if (string.IsNullOrWhiteSpace(selectedCategory))
            return;

        var categoryNode = tree.RootNodes.FirstOrDefault(n => n.Type == CatalogNodeType.Category && string.Equals(n.Title, selectedCategory, StringComparison.OrdinalIgnoreCase));
        if (categoryNode is null)
            return;

        categoryNode.IsExpanded = true;
        if (string.IsNullOrWhiteSpace(selectedShop))
        {
            tree.SelectedNode = categoryNode;
            return;
        }

        var shopNode = FindShopNode(categoryNode, selectedShop);
        tree.SelectedNode = shopNode ?? categoryNode;
    }

    private static CatalogNode? FindShopNode(CatalogNode root, string selectedShop)
    {
        foreach (var child in root.Children)
        {
            if (child.Type == CatalogNodeType.Shop && string.Equals(child.EditorShopName, selectedShop, StringComparison.OrdinalIgnoreCase))
                return child;

            var nested = FindShopNode(child, selectedShop);
            if (nested is not null)
                return nested;
        }

        return null;
    }
}

internal static class CatalogTreeState
{
    internal static Dictionary<string, bool> CaptureExpandedState(IEnumerable<CatalogNode> nodes)
    {
        var map = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (var node in nodes)
            Capture(node, BuildNodeKey(node), map);

        return map;
    }

    internal static void ApplyExpandedState(IEnumerable<CatalogNode> nodes, IReadOnlyDictionary<string, bool> map)
    {
        foreach (var node in nodes)
            Apply(node, BuildNodeKey(node), map);
    }

    private static void Capture(CatalogNode node, string path, IDictionary<string, bool> map)
    {
        map[path] = node.IsExpanded;
        foreach (var child in node.Children)
            Capture(child, $"{path}/{BuildNodeKey(child)}", map);
    }

    private static void Apply(CatalogNode node, string path, IReadOnlyDictionary<string, bool> map)
    {
        if (map.TryGetValue(path, out var expanded))
            node.IsExpanded = expanded;

        foreach (var child in node.Children)
            Apply(child, $"{path}/{BuildNodeKey(child)}", map);
    }

    private static string BuildNodeKey(CatalogNode node)
    {
        return node.Type switch
        {
            CatalogNodeType.Category => $"Category:{node.Title}",
            CatalogNodeType.Shop => $"Shop:{node.EditorShopName}",
            CatalogNodeType.ShopSegment => $"Segment:{node.EditorShopName}:{node.Title}",
            CatalogNodeType.Group => $"Group:{node.Title}",
            _ => $"Node:{node.Title}"
        };
    }
}
