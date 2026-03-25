namespace MerchantsPlus.Generator;

public abstract class OverviewViewModel : MerchantRulesViewModel
{
    private enum CenterPanelMode
    {
        ItemList,
        CatalogOverview,
        MerchantOverview
    }

    private CenterPanelMode _centerPanelMode = CenterPanelMode.ItemList;

    public bool ShowCenterItemList => _centerPanelMode == CenterPanelMode.ItemList;
    public bool ShowCenterOverviewTree => _centerPanelMode is CenterPanelMode.CatalogOverview or CenterPanelMode.MerchantOverview;
    public bool IsMerchantOverviewMode => _centerPanelMode == CenterPanelMode.MerchantOverview;
    public bool IsCatalogOverviewMode => _centerPanelMode == CenterPanelMode.CatalogOverview;
    public string CenterPanelTitle => _centerPanelMode switch
    {
        CenterPanelMode.CatalogOverview => "Item Catalog Overview",
        CenterPanelMode.MerchantOverview => "Merchant Overview",
        _ => GetItemViewerTitle()
    };

    public void ShowItemViewerInCenter()
    {
        SetCenterPanelMode(CenterPanelMode.ItemList);
        OnPropertyChanged(nameof(CenterPanelTitle));
    }

    public void ViewMerchantNode(MerchantTreeNode node)
    {
        if (string.IsNullOrWhiteSpace(node.CategoryName))
            return;

        var categoryNode = RootNodes.FirstOrDefault(n => n.Type == CatalogNodeType.Category && string.Equals(n.Title, node.CategoryName, StringComparison.OrdinalIgnoreCase));
        if (categoryNode is null)
        {
            EditorStatus = $"Could not find category {node.CategoryName} in item catalog.";
            return;
        }

        categoryNode.IsExpanded = true;
        if (string.IsNullOrWhiteSpace(node.ShopName))
        {
            SelectedNode = categoryNode;
            return;
        }

        var shopNode = FindCatalogShopNode(categoryNode, node.ShopName);
        SelectedNode = shopNode ?? categoryNode;
    }

    public void SetMerchantViewerExpanded(bool expanded)
    {
        foreach (var node in CenterOverviewNodes)
            SetExpanded(node, expanded);
    }

    public void SetMerchantOverviewAssignedExpanded(bool expanded)
    {
        foreach (var node in CenterOverviewNodes)
            SetExpanded(node, expanded);
    }

    public void SetMerchantOverviewUnassignedExpanded(bool expanded)
    {
        foreach (var node in CenterUnassignedOverviewNodes)
            SetExpanded(node, expanded);
    }

    public void SetMerchantEditorExpanded(bool expanded)
    {
        foreach (var node in MerchantAssignmentAvailableTree)
            SetExpanded(node, expanded);

        foreach (var node in MerchantAssignmentAssignedTree)
            SetExpanded(node, expanded);
    }

    public void SetMerchantEditorAvailableExpanded(bool expanded)
    {
        foreach (var node in MerchantAssignmentAvailableTree)
            SetExpanded(node, expanded);
    }

    public void SetMerchantEditorAssignedExpanded(bool expanded)
    {
        foreach (var node in MerchantAssignmentAssignedTree)
            SetExpanded(node, expanded);
    }

    public void ShowCatalogOverviewInCenter()
    {
        var expandedState = OverviewTreeState.CaptureExpandedState(CenterOverviewNodes);
        CenterOverviewNodes.Clear();
        CenterUnassignedOverviewNodes.Clear();

        foreach (var category in ItemCatalog.Categories.OrderBy(c => c.Name, StringComparer.OrdinalIgnoreCase))
        {
            var categoryNode = new MerchantTreeNode(category.Name);
            var orderedShops = category.Shops.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ToList();
            var shopMap = orderedShops.ToDictionary(shop => shop.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var shop in orderedShops.Where(shop => string.IsNullOrWhiteSpace(shop.ParentShopName) || !shopMap.ContainsKey(shop.ParentShopName)))
                categoryNode.Children.Add(BuildCatalogOverviewShopNode(shop, orderedShops));

            CenterOverviewNodes.Add(categoryNode);
        }

        if (CenterOverviewNodes.Count == 0)
            CenterOverviewNodes.Add(new MerchantTreeNode("No categories available."));

        OverviewTreeState.ApplyExpandedState(CenterOverviewNodes, expandedState);
        SetCenterPanelMode(CenterPanelMode.CatalogOverview);
    }

    public void ShowMerchantOverviewInCenter()
    {
        if (string.IsNullOrWhiteSpace(SelectedMerchantName) && MerchantNpcIds.Count > 0)
            SelectedMerchantName = MerchantNpcIds[0];

        var expandedState = OverviewTreeState.CaptureExpandedState(CenterOverviewNodes);
        var unassignedExpandedState = OverviewTreeState.CaptureExpandedState(CenterUnassignedOverviewNodes);
        CenterOverviewNodes.Clear();
        CenterUnassignedOverviewNodes.Clear();

        var assignedMerchants = MerchantAssignments
            .Where(entry => entry.Value.Categories.Count > 0 || entry.Value.Shops.Count > 0)
            .Select(entry => entry.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in MerchantAssignments.OrderBy(v => v.Key, StringComparer.OrdinalIgnoreCase))
        {
            var merchantNode = new MerchantTreeNode(entry.Key);
            var assignment = BuildEffectiveAssignment(entry.Value);
            var shopsByCategory = BuildShopsByCategory(assignment.Shops);

            var categoryNames = assignment.Categories
                .Concat(shopsByCategory.Keys)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(category => category, StringComparer.OrdinalIgnoreCase);

            foreach (var category in categoryNames)
            {
                var categoryNode = new MerchantTreeNode(category, categoryName: category);

                if (shopsByCategory.TryGetValue(category, out var shops))
                {
                    foreach (var shop in shops)
                        categoryNode.Children.Add(new MerchantTreeNode(shop, categoryName: category, shopName: shop));
                }

                merchantNode.Children.Add(categoryNode);
            }

            if (merchantNode.Children.Count > 0)
                CenterOverviewNodes.Add(merchantNode);
        }

        foreach (var merchant in MerchantNpcIds.OrderBy(name => name, StringComparer.OrdinalIgnoreCase))
        {
            if (!assignedMerchants.Contains(merchant))
                CenterUnassignedOverviewNodes.Add(new MerchantTreeNode(merchant));
        }

        if (CenterOverviewNodes.Count == 0)
            CenterOverviewNodes.Add(new MerchantTreeNode("No merchant assignments available."));

        if (CenterUnassignedOverviewNodes.Count == 0)
            CenterUnassignedOverviewNodes.Add(new MerchantTreeNode("No unassigned merchants."));

        OverviewTreeState.ApplyExpandedState(CenterOverviewNodes, expandedState);
        OverviewTreeState.ApplyExpandedState(CenterUnassignedOverviewNodes, unassignedExpandedState);
        SetCenterPanelMode(CenterPanelMode.MerchantOverview);
    }

    public void SelectMerchantFromOverviewNode(MerchantTreeNode node)
    {
        if (_centerPanelMode != CenterPanelMode.MerchantOverview || node.Children.Count == 0)
            return;

        SelectedMerchantName = node.Title;
    }

    protected override void OnSelectedMerchantChanged()
    {
        NotifyOverrideAvailabilityChanged(includeCategoryLevel: true);
        RefreshMerchantEditorOptions();
        RefreshMerchantViewer();
        RefreshMerchantWorkspaceEditor();
        RefreshCenterOverviewIfNeeded();
    }

    protected override void OnSelectedNodeChanged()
    {
        base.OnSelectedNodeChanged();
        SetCenterPanelMode(CenterPanelMode.ItemList);
        OnPropertyChanged(nameof(CenterPanelTitle));
        NotifyOverrideAvailabilityChanged(includeCategoryLevel: true);
    }

    protected override void OnSelectedCategoryChanged()
    {
        base.OnSelectedCategoryChanged();
        NotifyOverrideAvailabilityChanged(includeCategoryLevel: true);
    }

    protected override void OnSelectedShopChanged()
    {
        base.OnSelectedShopChanged();
        NotifyOverrideAvailabilityChanged(includeCategoryLevel: false);
    }

    protected override void RefreshCenterOverviewIfNeeded()
    {
        if (_centerPanelMode == CenterPanelMode.CatalogOverview)
            ShowCatalogOverviewInCenter();
        else if (_centerPanelMode == CenterPanelMode.MerchantOverview)
            ShowMerchantOverviewInCenter();
    }

    protected string GetItemViewerTitle()
    {
        if (SelectedNode is null)
            return SelectedTitle;

        if (SelectedNode.Type is not (CatalogNodeType.Shop or CatalogNodeType.ShopSegment))
            return SelectedNode.Type == CatalogNodeType.Category ? SelectedNode.Title : SelectedTitle;

        var category = RootNodes.FirstOrDefault(n => n.Type == CatalogNodeType.Category && ContainsNode(n, SelectedNode));
        return category is null ? SelectedNode.EditorShopName : $"{category.Title} / {SelectedNode.EditorShopName}";
    }

    private void SetCenterPanelMode(CenterPanelMode mode)
    {
        if (_centerPanelMode == mode)
            return;

        _centerPanelMode = mode;
        OnPropertyChanged(nameof(ShowCenterItemList));
        OnPropertyChanged(nameof(ShowCenterOverviewTree));
        OnPropertyChanged(nameof(IsMerchantOverviewMode));
        OnPropertyChanged(nameof(IsCatalogOverviewMode));
        OnPropertyChanged(nameof(CenterPanelTitle));
    }

    private static CatalogNode? FindCatalogShopNode(CatalogNode root, string shopName)
    {
        foreach (var child in root.Children)
        {
            if (child.Type == CatalogNodeType.Shop && string.Equals(child.EditorShopName, shopName, StringComparison.OrdinalIgnoreCase))
                return child;

            var nested = FindCatalogShopNode(child, shopName);
            if (nested is not null)
                return nested;
        }

        return null;
    }

    private static MerchantTreeNode BuildCatalogOverviewShopNode(Shop shop, IReadOnlyList<Shop> orderedShops)
    {
        var node = new MerchantTreeNode(shop.Name);
        foreach (var child in orderedShops.Where(candidate => string.Equals(candidate.ParentShopName, shop.Name, StringComparison.OrdinalIgnoreCase)))
            node.Children.Add(BuildCatalogOverviewShopNode(child, orderedShops));

        return node;
    }

    private static Dictionary<string, string[]> BuildShopsByCategory(IEnumerable<string> shopKeys)
    {
        return shopKeys
            .Select(shopKey => TryParseShopKey(shopKey, out var categoryName, out var shopName)
                ? (Category: categoryName, Shop: shopName, IsValid: true)
                : (Category: string.Empty, Shop: string.Empty, IsValid: false))
            .Where(tuple => tuple.IsValid)
            .GroupBy(tuple => tuple.Category, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                group => group.Key,
                group => group
                    .Select(tuple => tuple.Shop)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(shop => shop, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                StringComparer.OrdinalIgnoreCase);
    }

    private void NotifyOverrideAvailabilityChanged(bool includeCategoryLevel)
    {
        OnPropertyChanged("CanResetSelectedShopPriceOverrides");
        OnPropertyChanged("CanResetSelectedShopConditionOverrides");

        if (!includeCategoryLevel)
            return;

        OnPropertyChanged("CanResetSelectedCategoryPriceOverrides");
        OnPropertyChanged("CanResetSelectedCategoryConditionOverrides");
    }

}
