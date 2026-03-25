namespace MerchantsPlus.Generator;

public abstract class CatalogTree : CatalogTreeHelpers
{
    protected override void Load(IReadOnlyList<Category> categories, IReadOnlyList<string> unorganized)
    {
        RootNodes.Clear();
        VisibleItems.Clear();
        CategoryCount = 0;
        ShopCount = 0;
        ItemCount = 0;

        foreach (var category in categories)
        {
            var categoryNode = new CatalogNode(category.Name, CatalogNodeType.Category);
            var orderedShops = category.Shops.ToList();
            var shopMap = orderedShops.ToDictionary(shop => shop.Name, StringComparer.OrdinalIgnoreCase);
            var shopCount = orderedShops.Count;
            var itemCount = orderedShops.Sum(shop => shop.Items.Count);

            categoryNode.ShopCountBadgeText = shopCount == 1 ? "1 Shop" : $"{shopCount} Shops";
            categoryNode.ItemCountBadgeText = itemCount == 1 ? "1 Item" : $"{itemCount} Items";

            foreach (var shop in orderedShops.Where(shop => IsRootShop(shop, shopMap)))
                categoryNode.Children.Add(BuildShopNode(shop, orderedShops));

            ShopCount += shopCount;
            ItemCount += itemCount;

            RootNodes.Add(categoryNode);
            CategoryCount++;
        }

        if (unorganized.Count > 0)
        {
            RootNodes.Add(new CatalogNode($"Unorganized ({unorganized.Count})", CatalogNodeType.Group, unorganized));
            ItemCount += unorganized.Count;
        }

        RefreshFilteredRootNodes();

        RefreshCategoryOptions();
        OnPropertyChanged(nameof(CategoryCount));
        OnPropertyChanged(nameof(ShopCount));
        OnPropertyChanged(nameof(ItemCount));
        OnPropertyChanged(nameof(HeaderStats));
        NotifyWorkspaceStatsChanged();
        RefreshBlacklistedUnorganizedItems();
        OnCatalogLoaded();

        SelectedNode = FilteredRootNodes.Count > 0 ? (FilteredRootNodes[0].Children.FirstOrDefault() ?? FilteredRootNodes[0]) : null;
    }

    protected override void RefreshCategoryOptions()
    {
        CategoryOptions.Clear();
        foreach (var category in ItemCatalog.Categories)
            CategoryOptions.Add(category.Name);

        if (!string.IsNullOrWhiteSpace(SelectedCategoryForShop) && CategoryOptions.Contains(SelectedCategoryForShop))
        {
            RefreshShopOptions();
            return;
        }

        SelectedCategoryForShop = CategoryOptions.FirstOrDefault() ?? string.Empty;
        RefreshShopOptions();
    }

    protected override void RefreshShopOptions()
    {
        ShopOptions.Clear();

        var category = ItemCatalog.Categories.FirstOrDefault(c => string.Equals(c.Name, SelectedCategoryForShop, StringComparison.OrdinalIgnoreCase));
        if (category is null)
        {
            SelectedShopName = string.Empty;
            ClearShopInputs();
            return;
        }

        foreach (var shop in category.Shops)
            ShopOptions.Add(shop.Name);

        if (!string.IsNullOrWhiteSpace(SelectedShopName) && ShopOptions.Contains(SelectedShopName))
            return;

        SelectedShopName = ShopOptions.FirstOrDefault() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(SelectedShopName))
            ClearShopInputs();
    }

    protected override void OnSelectedCategoryChanged()
    {
        if (!string.IsNullOrWhiteSpace(SelectedCategoryForShop))
            LoadSelectedCategoryInputs();

        RefreshShopOptions();
    }

    protected override void OnSelectedShopChanged()
    {
        var category = ItemCatalog.Categories.FirstOrDefault(c => string.Equals(c.Name, SelectedCategoryForShop, StringComparison.OrdinalIgnoreCase));
        var shop = category?.Shops.FirstOrDefault(s => string.Equals(s.Name, SelectedShopName, StringComparison.OrdinalIgnoreCase));

        if (shop is null)
        {
            ClearShopInputs();
            return;
        }

        NewShopName = shop.Name;
        NewShopPriority = shop.Priority.ToString();
        LoadRuleRows(ShopKeywordRules, shop.KeywordRules);
        NewShopRuleMode = KeywordRule.WhitelistMode;
        NewShopRuleMatch = KeywordRule.ContainsMatch;
        NewShopRuleTerm = string.Empty;
        NewShopFirstOrder = string.Join(", ", shop.FirstOrder);
        NewShopLastOrder = string.Join(", ", shop.LastOrder);
    }

    protected override void OnSelectedNodeChanged()
    {
        SyncEditorSelectionFromTree();
        RefreshVisibleItems();

        if (_selectedNode?.Type == CatalogNodeType.Group)
            LoadUnorganizedBlacklistInputs();
    }

    private void SyncEditorSelectionFromTree()
    {
        if (_selectedNode is null)
            return;

        if (_selectedNode.Type == CatalogNodeType.Category)
        {
            SelectedCategoryForShop = _selectedNode.Title;
            SelectedShopName = string.Empty;
            RightEditorTabIndex = 0;
            return;
        }

        if (_selectedNode.Type is CatalogNodeType.Shop or CatalogNodeType.ShopSegment)
        {
            var category = RootNodes.FirstOrDefault(root => root.Type == CatalogNodeType.Category && ContainsNode(root, _selectedNode));
            if (category is not null)
                SelectedCategoryForShop = category.Title;

            SelectedShopName = _selectedNode.EditorShopName;
            RightEditorTabIndex = 1;
            return;
        }

        SelectedCategoryForShop = string.Empty;
        SelectedShopName = string.Empty;
        NewCategoryName = string.Empty;
        CategoryKeywordRules.Clear();
        NewCategoryRuleMode = KeywordRule.WhitelistMode;
        NewCategoryRuleMatch = KeywordRule.ContainsMatch;
        NewCategoryRuleTerm = string.Empty;
        NewCategoryFirstOrder = string.Empty;
        NewCategoryLastOrder = string.Empty;
        ClearShopInputs();
    }

    protected override void RefreshVisibleItems()
    {
        VisibleItems.Clear();
        if (_selectedNode is null)
            return;

        var rows = new List<(CatalogItemRow Row, int Index)>();
        var index = 0;

        foreach (var item in _selectedNode.Items)
        {
            if (string.IsNullOrWhiteSpace(_filterText) || item.Contains(_filterText, StringComparison.OrdinalIgnoreCase))
            {
                rows.Add((CreateItemRow(item), index));
                index++;
            }
        }

        if (_selectedNode.Type == CatalogNodeType.Shop)
        {
            foreach (var nestedShop in _selectedNode.Children.Where(child => child.Type == CatalogNodeType.Shop))
            {
                if (string.IsNullOrWhiteSpace(_filterText) || nestedShop.EditorShopName.Contains(_filterText, StringComparison.OrdinalIgnoreCase))
                {
                    rows.Add((CreateItemRow(nestedShop.EditorShopName, isShopSlot: true), index));
                    index++;
                }
            }
        }

        foreach (var row in rows.OrderBy(v => GetSortBucket(v.Row)).ThenBy(v => v.Index))
            VisibleItems.Add(row.Row);
    }

    private static int GetSortBucket(CatalogItemRow row)
    {
        if (string.Equals(row.SortFlag, "First", StringComparison.OrdinalIgnoreCase))
            return 0;

        if (string.Equals(row.SortFlag, "Last", StringComparison.OrdinalIgnoreCase))
            return 2;

        return 1;
    }

    protected override void ReloadFromCatalogPreservingSelection()
    {
        var expandedState = CatalogTreeState.CaptureExpandedState(RootNodes);
        var selectedCategory = SelectedCategoryForShop;
        var selectedShop = SelectedShopName;

        Load(ItemCatalog.Categories, ItemCatalog.Unorganized);
        CatalogTreeState.ApplyExpandedState(RootNodes, expandedState);

        if (!string.IsNullOrWhiteSpace(selectedCategory) && CategoryOptions.Contains(selectedCategory))
            SelectedCategoryForShop = selectedCategory;

        if (!string.IsNullOrWhiteSpace(selectedShop) && ShopOptions.Contains(selectedShop))
            SelectedShopName = selectedShop;
    }

    protected override void RestoreSelectionAfterReload(string selectedCategory, string selectedShop)
    {
        TreeSelectionRestorer.RestoreSelection(this, selectedCategory, selectedShop);
    }

    private static bool IsRootShop(Shop shop, IReadOnlyDictionary<string, Shop> shopMap)
    {
        if (string.IsNullOrWhiteSpace(shop.ParentShopName))
            return true;

        return !shopMap.ContainsKey(shop.ParentShopName);
    }

    private CatalogNode BuildShopNode(Shop shop, IReadOnlyList<Shop> orderedShops)
    {
        var nestedShops = orderedShops
            .Where(candidate => string.Equals(candidate.ParentShopName, shop.Name, StringComparison.OrdinalIgnoreCase))
            .ToList();

        var slotCount = shop.Items.Count + nestedShops.Count;
        var shopNode = new CatalogNode(shop.Name, CatalogNodeType.Shop, shop.Items, shop.Name, slotCount);

        if (shop.Items.Count > MaxItemsPerShopChunk)
        {
            var chunkIndex = 0;
            for (var i = 0; i < shop.Items.Count; i += MaxItemsPerShopChunk)
            {
                var chunkItems = shop.Items.Skip(i).Take(MaxItemsPerShopChunk).ToArray();
                var chunkName = $"{shop.Name} {chunkIndex + 1}";
                shopNode.Children.Add(new CatalogNode(chunkName, CatalogNodeType.ShopSegment, chunkItems, shop.Name));
                chunkIndex++;
            }
        }

        foreach (var nestedShop in nestedShops)
            shopNode.Children.Add(BuildShopNode(nestedShop, orderedShops));

        return shopNode;
    }

}
