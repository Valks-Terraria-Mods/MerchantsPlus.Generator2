using System.Collections.ObjectModel;

namespace MerchantsPlus.Generator;

public abstract class MerchantBaseViewModel : CatalogTree
{
    private const string ShopKeySeparator = " / ";

    protected readonly Dictionary<string, MerchantAssignment> MerchantAssignments = new(StringComparer.OrdinalIgnoreCase);
    protected bool IsMerchantSelectionSync;
    protected bool MerchantAssignmentsLoaded;

    public override int TotalAssignedShopCount => MerchantAssignments.Values
        .Select(BuildEffectiveAssignment)
        .SelectMany(assignment => assignment.Shops)
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .Count();

    public override int TotalUnassignedShopCount
    {
        get
        {
            var totalAssignableShops = GetAssignableShops()
                .Select(tuple => BuildShopKey(tuple.Category.Name, tuple.Shop.Name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Count();
            return Math.Max(0, totalAssignableShops - TotalAssignedShopCount);
        }
    }

    public ObservableCollection<MerchantTreeNode> CenterOverviewNodes { get; } = [];
    public ObservableCollection<MerchantTreeNode> CenterUnassignedOverviewNodes { get; } = [];

    protected void LoadMerchants()
    {
        MerchantNpcIds.Clear();

        try
        {
            foreach (var npc in NpcIds.GetTownNpcIds())
                MerchantNpcIds.Add(npc);

            RefreshFilteredMerchantNpcIds();
            NotifyWorkspaceStatsChanged();
        }
        catch (Exception ex)
        {
            EditorStatus = $"Failed to load town NPC IDs: {ex.Message}";
        }
    }

    protected void LoadMerchantAssignments()
    {
        MerchantAssignments.Clear();

        foreach (var config in CatalogStorage.LoadMerchantAssignmentConfigs())
        {
            if (string.IsNullOrWhiteSpace(config.MerchantName))
                continue;

            var assignment = new MerchantAssignment();
            foreach (var category in config.Categories)
                assignment.Categories.Add(category);

            foreach (var shop in config.Shops)
                assignment.Shops.Add(shop);

            MerchantAssignments[config.MerchantName] = assignment;
        }

        NormalizeMerchantAssignments();
        MerchantAssignmentsLoaded = true;
        SaveMerchantAssignmentsAndOutput();
        NotifyWorkspaceStatsChanged();
    }

    protected void SaveMerchantAssignmentsAndOutput()
    {
        if (!MerchantAssignmentsLoaded)
            return;

        var configs = MerchantAssignments
            .OrderBy(v => v.Key, StringComparer.OrdinalIgnoreCase)
            .Select(v =>
            {
                var effective = BuildEffectiveAssignment(v.Value);
                return new MerchantAssignmentConfig
                {
                    MerchantName = v.Key,
                    Categories = effective.Categories.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray(),
                    Shops = effective.Shops.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).ToArray()
                };
            })
            .Where(v => v.Categories.Length > 0 || v.Shops.Length > 0)
            .ToList();

        CatalogStorage.SaveMerchantAssignmentConfigs(configs);
        CatalogStorage.SaveMerchantOutputs(BuildMerchantOutputs());
        RefreshOutputFiles();
        NotifyWorkspaceStatsChanged();
    }

    protected MerchantAssignment GetOrCreateAssignment(string merchantName)
    {
        if (MerchantAssignments.TryGetValue(merchantName, out var assignment))
            return assignment;

        assignment = new MerchantAssignment();
        MerchantAssignments[merchantName] = assignment;
        return assignment;
    }

    protected MerchantAssignment BuildEffectiveAssignment(MerchantAssignment source)
    {
        var effective = new MerchantAssignment();

        foreach (var categoryName in source.Categories)
        {
            var category = ItemCatalog.Categories.FirstOrDefault(v => string.Equals(v.Name, categoryName, StringComparison.OrdinalIgnoreCase));
            if (category is null)
                continue;

            var shopMap = category.Shops.ToDictionary(shop => shop.Name, StringComparer.OrdinalIgnoreCase);
            foreach (var shop in category.Shops.Where(shop => IsAssignableShop(shop, shopMap)))
                effective.Shops.Add(BuildShopKey(category.Name, shop.Name));
        }

        foreach (var shop in source.Shops)
        {
            if (TryGetAssignableShop(shop, out var resolvedCategory, out var resolvedShop))
                effective.Shops.Add(BuildShopKey(resolvedCategory.Name, resolvedShop.Name));
        }

        return effective;
    }

    protected static string BuildShopKey(string categoryName, string shopName)
    {
        return $"{categoryName}{ShopKeySeparator}{shopName}";
    }

    protected IEnumerable<string> GetShopKeysForCategory(string categoryName)
    {
        var category = ItemCatalog.Categories.FirstOrDefault(c => string.Equals(c.Name, categoryName, StringComparison.OrdinalIgnoreCase));
        if (category is null)
            return [];

        var shopMap = category.Shops.ToDictionary(shop => shop.Name, StringComparer.OrdinalIgnoreCase);
        return category.Shops
            .Where(shop => IsAssignableShop(shop, shopMap))
            .Select(shop => BuildShopKey(category.Name, shop.Name));
    }

    protected IEnumerable<(Category Category, Shop Shop)> GetAssignableShops()
    {
        foreach (var category in ItemCatalog.Categories.OrderBy(v => v.Name, StringComparer.OrdinalIgnoreCase))
        {
            var shopMap = category.Shops.ToDictionary(shop => shop.Name, StringComparer.OrdinalIgnoreCase);
            foreach (var shop in category.Shops
                .Where(candidate => IsAssignableShop(candidate, shopMap))
                .OrderBy(candidate => candidate.Name, StringComparer.OrdinalIgnoreCase))
            {
                yield return (category, shop);
            }
        }
    }

    protected bool TryGetAssignableShop(string shopKey, out Category category, out Shop shop)
    {
        category = null!;
        shop = null!;

        if (!TryParseShopKey(shopKey, out var categoryName, out var shopName))
            return false;

        category = ItemCatalog.Categories.FirstOrDefault(candidate => string.Equals(candidate.Name, categoryName, StringComparison.OrdinalIgnoreCase))!;
        if (category is null)
            return false;

        var shopMap = category.Shops.ToDictionary(candidate => candidate.Name, StringComparer.OrdinalIgnoreCase);
        if (!shopMap.TryGetValue(shopName, out shop!))
            return false;

        return IsAssignableShop(shop, shopMap);
    }

    protected override bool MatchesMerchantFilter(string npc, string filter)
    {
        if (npc.Contains(filter, StringComparison.OrdinalIgnoreCase))
            return true;

        if (!MerchantAssignments.TryGetValue(npc, out var assignment))
            return false;

        var effective = BuildEffectiveAssignment(assignment);
        foreach (var shopKey in effective.Shops)
        {
            if (!TryGetAssignableShop(shopKey, out var category, out var shop))
                continue;

            if (ShopMatchesFilter(category, shop, filter, new HashSet<string>(StringComparer.OrdinalIgnoreCase)))
                return true;
        }

        return false;
    }

    protected static IEnumerable<ShopOutput> SplitShopForOutput(Category category, Shop shop)
    {
        return StorageShopOutputBuilder.BuildShopOutputs(category, shop);
    }

    protected static void SetExpanded(MerchantTreeNode node, bool expanded)
    {
        node.IsExpanded = expanded;
        foreach (var child in node.Children)
            SetExpanded(child, expanded);
    }

    protected static void SetExpanded(AssignmentTreeNode node, bool expanded)
    {
        node.IsExpanded = expanded;
        foreach (var child in node.Children)
            SetExpanded(child, expanded);
    }

    public void ForceRefreshAllJsonOutputs()
    {
        RebuildCatalog(forceRefresh: true);
        CatalogStorage.SaveCategoryConfigs(ItemCatalogConfiguration.ToConfigs(ItemCatalog.Categories));
        CatalogStorage.SaveCatalogOutput(ItemCatalog.Categories);
        SaveMerchantAssignmentsAndOutput();
        CatalogStorage.SaveGlobalFlagsConfig(ItemCatalog.GetGlobalFlagsConfig());
        CatalogStorage.SaveUnorganizedBlacklistConfig(ItemCatalog.GetUnorganizedBlacklistConfig());
        EditorStatus = "Force refreshed all JSON output files.";
    }

    private List<MerchantOutput> BuildMerchantOutputs()
    {
        var outputs = new List<MerchantOutput>();

        foreach (var entry in MerchantAssignments.OrderBy(v => v.Key, StringComparer.OrdinalIgnoreCase))
        {
            var assignment = BuildEffectiveAssignment(entry.Value);
            var shops = new List<ShopOutput>();

            foreach (var category in ItemCatalog.Categories)
            {
                var hasCategory = assignment.Categories.Contains(category.Name);
                var selectedShops = category.Shops
                    .Where(shop => assignment.Shops.Contains(BuildShopKey(category.Name, shop.Name)))
                    .ToList();

                if (!hasCategory && selectedShops.Count == 0)
                    continue;

                shops.AddRange(StorageShopOutputBuilder.BuildTopLevelShopOutputs(category, selectedShops));
            }

            if (shops.Count == 0)
                continue;

            outputs.Add(new MerchantOutput { Name = entry.Key, Shops = shops });
        }

        return outputs;
    }

    private void NormalizeMerchantAssignments()
    {
        foreach (var assignment in MerchantAssignments.Values)
        {
            var effective = BuildEffectiveAssignment(assignment);
            assignment.Categories.Clear();
            assignment.Shops.Clear();

            foreach (var category in effective.Categories)
                assignment.Categories.Add(category);

            foreach (var shop in effective.Shops)
                assignment.Shops.Add(shop);
        }
    }

    protected static bool TryParseShopKey(string shopKey, out string categoryName, out string shopName)
    {
        categoryName = string.Empty;
        shopName = string.Empty;

        if (string.IsNullOrWhiteSpace(shopKey))
            return false;

        var separatorIndex = shopKey.IndexOf(ShopKeySeparator, StringComparison.Ordinal);
        if (separatorIndex <= 0 || separatorIndex + ShopKeySeparator.Length >= shopKey.Length)
            return false;

        categoryName = shopKey[..separatorIndex];
        shopName = shopKey[(separatorIndex + ShopKeySeparator.Length)..];
        return !string.IsNullOrWhiteSpace(categoryName) && !string.IsNullOrWhiteSpace(shopName);
    }

    private static bool IsAssignableShop(Shop shop, IReadOnlyDictionary<string, Shop> shopMap)
    {
        if (string.IsNullOrWhiteSpace(shop.ParentShopName))
            return true;

        return !shopMap.ContainsKey(shop.ParentShopName);
    }

    private static bool ShopMatchesFilter(Category category, Shop shop, string filter, HashSet<string> visited)
    {
        if (!visited.Add(shop.Name))
            return false;

        if (shop.Name.Contains(filter, StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (var item in shop.Items)
        {
            if (item.Contains(filter, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        foreach (var nestedShop in category.Shops.Where(candidate => string.Equals(candidate.ParentShopName, shop.Name, StringComparison.OrdinalIgnoreCase)))
        {
            if (ShopMatchesFilter(category, nestedShop, filter, visited))
                return true;
        }

        return false;
    }
}
