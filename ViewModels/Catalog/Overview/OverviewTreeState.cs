namespace MerchantsPlus.Generator;

internal static class OverviewTreeState
{
    internal static Dictionary<string, bool> CaptureExpandedState(IEnumerable<MerchantTreeNode> nodes)
    {
        var map = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        foreach (var node in nodes)
            Capture(node, node.Title, map);

        return map;
    }

    internal static void ApplyExpandedState(IEnumerable<MerchantTreeNode> nodes, IReadOnlyDictionary<string, bool> map)
    {
        foreach (var node in nodes)
            Apply(node, node.Title, map);
    }

    private static void Capture(MerchantTreeNode node, string path, IDictionary<string, bool> map)
    {
        map[path] = node.IsExpanded;
        foreach (var child in node.Children)
            Capture(child, $"{path}/{child.Title}", map);
    }

    private static void Apply(MerchantTreeNode node, string path, IReadOnlyDictionary<string, bool> map)
    {
        if (map.TryGetValue(path, out var expanded))
            node.IsExpanded = expanded;

        foreach (var child in node.Children)
            Apply(child, $"{path}/{child.Title}", map);
    }
}
