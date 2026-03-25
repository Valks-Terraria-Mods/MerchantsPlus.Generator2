using System.Collections.ObjectModel;

namespace MerchantsPlus.Generator;

public abstract class MerchantEditorViewModel : MerchantViewerViewModel
{
    private readonly MerchantAssignmentActionsComponent _assignmentActionsComponent;
    private readonly MerchantSelectionSetComponent _selectionSetComponent;
    private readonly MerchantEditorOptionsComponent _editorOptionsComponent;
    private readonly MerchantOptionWiringComponent _optionWiringComponent;
    private readonly MerchantWorkspaceComponent _workspaceComponent;
    private readonly MerchantCategorySuffixComponent _categorySuffixComponent;

    protected MerchantEditorViewModel()
        : this(new MerchantAssignmentActionsComponent(), new MerchantSelectionSetComponent(), new MerchantEditorOptionsComponent(), new MerchantOptionWiringComponent(), new MerchantWorkspaceComponent(), new MerchantCategorySuffixComponent()) { }

    protected MerchantEditorViewModel(
        MerchantAssignmentActionsComponent assignmentActionsComponent,
        MerchantSelectionSetComponent selectionSetComponent,
        MerchantEditorOptionsComponent editorOptionsComponent,
        MerchantOptionWiringComponent optionWiringComponent,
        MerchantWorkspaceComponent workspaceComponent,
        MerchantCategorySuffixComponent categorySuffixComponent)
    {
        _assignmentActionsComponent = assignmentActionsComponent;
        _selectionSetComponent = selectionSetComponent;
        _editorOptionsComponent = editorOptionsComponent;
        _optionWiringComponent = optionWiringComponent;
        _workspaceComponent = workspaceComponent;
        _categorySuffixComponent = categorySuffixComponent;
    }

    public void SaveSelectedMerchantAssignments()
    {
        var selectedCategories = _selectionSetComponent.BuildSelectedCategories(MerchantCategoryOptions);
        var selectedShops = _selectionSetComponent.BuildSelectedShops(MerchantShopOptions);
        if (!_assignmentActionsComponent.SaveSelectedAssignments(SelectedMerchantName, selectedCategories, selectedShops, FindConflictsWithOtherMerchants, GetOrCreateAssignment, out var assignmentHint, out var editorStatus))
        {
            if (!string.IsNullOrWhiteSpace(assignmentHint))
                MerchantAssignmentHint = assignmentHint;
            if (!string.IsNullOrWhiteSpace(editorStatus))
                EditorStatus = editorStatus;
            return;
        }

        MerchantAssignmentHint = assignmentHint;
        RefreshAfterAssignmentChanged(refreshCenterOverview: true);
        EditorStatus = editorStatus;
    }

    public void RemoveMerchantNode(MerchantTreeNode node)
    {
        if (!_assignmentActionsComponent.RemoveMerchantNode(SelectedMerchantName, node, GetOrCreateAssignment, GetShopKeysForCategory, BuildShopKey, out var editorStatus))
            return;

        RefreshAfterAssignmentChanged(refreshCenterOverview: true);
        EditorStatus = editorStatus;
    }

    public void SelectMerchantFromWorkspaceNode(MerchantTreeNode node)
        => SelectedMerchantName = string.IsNullOrWhiteSpace(node.Title) ? SelectedMerchantName : node.Title;

    public void AddSelectedMerchantShop()
    {
        if (!_assignmentActionsComponent.AddSelectedMerchantShop(SelectedMerchantName, SelectedMerchantAddShop, GetOrCreateAssignment, out var editorStatus))
            return;

        RefreshAfterAssignmentChanged(refreshCenterOverview: false);
        EditorStatus = editorStatus;
    }

    public void RemoveMerchantOwnedShop(MerchantShopBadgeRow row)
    {
        if (!_assignmentActionsComponent.RemoveMerchantOwnedShop(SelectedMerchantName, row, GetOrCreateAssignment, out var editorStatus))
            return;

        RefreshAfterAssignmentChanged(refreshCenterOverview: false);
        EditorStatus = editorStatus;
    }

    protected override void OnCatalogLoaded() { RefreshMerchantEditorOptions(); RefreshMerchantViewer(); RefreshMerchantWorkspace(); RefreshCenterOverviewIfNeeded(); }

    protected override void OnMerchantFilterChanged() => RefreshMerchantWorkspace();
    protected override void OnMerchantSortChanged() => RefreshMerchantWorkspace();

    protected void RefreshMerchantEditorOptions()
    {
        var selectedMerchant = SelectedMerchantName;
        var expandedState = _editorOptionsComponent.CaptureExpandedState(MerchantAssignmentOptionsTree, MerchantAssignmentAvailableTree, MerchantAssignmentAssignedTree);
        var assignment = string.IsNullOrWhiteSpace(selectedMerchant) ? new MerchantAssignment() : BuildEffectiveAssignment(GetOrCreateAssignment(selectedMerchant));
        var categoryOwners = string.IsNullOrWhiteSpace(selectedMerchant) ? new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase) : GetCategoryOwners(selectedMerchant);
        var shopOwners = string.IsNullOrWhiteSpace(selectedMerchant) ? new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase) : GetShopOwners(selectedMerchant);
        var result = _editorOptionsComponent.BuildOptions(
            selectedMerchant,
            ItemCatalog.Categories,
            assignment,
            categoryOwners,
            shopOwners,
            expandedState,
            BuildShopKey,
            IsAssignableShopKey,
            categoryName => _categorySuffixComponent.BuildCategoryMerchantSuffix(categoryName, MerchantAssignments, BuildEffectiveAssignment),
            UpdateCategoryStateFromChildren,
            WireShopOption,
            WireCategoryOption);

        ReplaceCollection(MerchantCategoryOptions, result.CategoryOptions);
        ReplaceCollection(MerchantShopOptions, result.ShopOptions);
        ReplaceCollection(MerchantAssignmentOptionsTree, result.OptionsTree);
        ReplaceCollection(MerchantAssignmentAvailableTree, result.AvailableTree);
        ReplaceCollection(MerchantAssignmentAssignedTree, result.AssignedTree);
        MerchantAssignmentHint = result.AssignmentHint;
    }

    public void RefreshMerchantWorkspace()
    {
        var result = _workspaceComponent.BuildWorkspaceNodes(
            MerchantNpcIds,
            MerchantFilterText,
            MerchantSortMode,
            MatchesMerchantFilter,
            ResolveEffectiveAssignmentForMerchant,
            ResolveAssignableShop,
            BuildShopKey,
            SelectedMerchantWorkspaceNode?.Title ?? SelectedMerchantName);

        ReplaceCollection(MerchantWorkspaceNodes, result.WorkspaceNodes);
        if (MerchantWorkspaceNodes.Count == 0)
        {
            SelectedMerchantWorkspaceNode = null;
            MerchantOwnedShopBadges.Clear();
            MerchantAvailableShops.Clear();
            SelectedMerchantAddShop = null;
            return;
        }

        SelectedMerchantWorkspaceNode = MerchantWorkspaceNodes.FirstOrDefault(v => string.Equals(v.Title, result.SelectedMerchantName, StringComparison.OrdinalIgnoreCase)) ?? MerchantWorkspaceNodes[0];
        RefreshSelectedMerchantWorkspaceEditor();
    }

    protected void RefreshMerchantWorkspaceEditor() => RefreshSelectedMerchantWorkspaceEditor();
    protected abstract void RefreshCenterOverviewIfNeeded();

    private void RefreshSelectedMerchantWorkspaceEditor()
    {
        if (string.IsNullOrWhiteSpace(SelectedMerchantName))
        {
            MerchantOwnedShopBadges.Clear();
            MerchantAvailableShops.Clear();
            SelectedMerchantAddShop = null;
            return;
        }

        var assignment = BuildEffectiveAssignment(GetOrCreateAssignment(SelectedMerchantName));
        var result = _workspaceComponent.BuildWorkspaceEditor(SelectedMerchantName, assignment.Shops, GetShopOwners(SelectedMerchantName), GetAssignableShops(), BuildShopKey, ResolveAssignableShop);
        ReplaceCollection(MerchantOwnedShopBadges, result.OwnedShopBadges);
        ReplaceCollection(MerchantAvailableShops, result.AvailableShopBadges);
        SelectedMerchantAddShop = result.SelectedAddShop;
    }

    private void RefreshAfterAssignmentChanged(bool refreshCenterOverview)
    {
        SaveMerchantAssignmentsAndOutput();
        RefreshMerchantEditorOptions();
        RefreshMerchantViewer();
        RefreshMerchantWorkspace();
        if (refreshCenterOverview)
            RefreshCenterOverviewIfNeeded();
    }

    private void WireShopOption(SelectableOption category, List<SelectableOption> children, SelectableOption shop)
        => _optionWiringComponent.WireShopOption(category, children, shop, () => IsMerchantSelectionSync, value => IsMerchantSelectionSync = value, UpdateCategoryStateFromChildren, SaveSelectedMerchantAssignments);

    private void WireCategoryOption(SelectableOption category, List<SelectableOption> children)
        => _optionWiringComponent.WireCategoryOption(category, children, () => IsMerchantSelectionSync, value => IsMerchantSelectionSync = value, UpdateCategoryStateFromChildren, SaveSelectedMerchantAssignments);

    private MerchantAssignment ResolveEffectiveAssignmentForMerchant(string merchantName)
        => MerchantAssignments.TryGetValue(merchantName, out var existing) ? BuildEffectiveAssignment(existing) : new MerchantAssignment();

    private (bool Success, Category Category, Shop Shop) ResolveAssignableShop(string shopKey)
        => TryGetAssignableShop(shopKey, out var category, out var shop) ? (true, category, shop) : (false, null!, null!);

    private bool IsAssignableShopKey(string shopKey) => TryGetAssignableShop(shopKey, out _, out _);
    protected virtual List<string> FindConflictsWithOtherMerchants(string merchantName, HashSet<string> categories, HashSet<string> shops) => [];
    protected virtual Dictionary<string, List<string>> GetCategoryOwners(string selectedMerchant) => new(StringComparer.OrdinalIgnoreCase);
    protected virtual Dictionary<string, List<string>> GetShopOwners(string selectedMerchant) => new(StringComparer.OrdinalIgnoreCase);
    protected virtual void UpdateCategoryStateFromChildren(SelectableOption category, IEnumerable<SelectableOption> children) { }

    private static void ReplaceCollection<T>(ObservableCollection<T> target, IEnumerable<T> values)
    {
        target.Clear();
        foreach (var value in values)
            target.Add(value);
    }
}
