namespace MerchantsPlus.Generator;

public abstract class CatalogStateEditorView : CatalogStateOutputSettingsView
{
    private static readonly IReadOnlyList<string> MiddlePanelSplitModes = ["Single View", "Split Vertically", "Split Horizontally"];

    protected CatalogStateEditorView() { }

    protected CatalogStateEditorView(CatalogStateComponents components) : base(components) { }

    public string SelectedTitle => Selection.SelectedNode is null ? "Select a shop in the tree" : Selection.SelectedNode.DisplayName;
    public string MiddlePanelTitle => SelectedTitle;
    public string PrimaryPanelTitle => BuildShopViewTitle(SelectedCategoryForShop, SelectedShopName, SelectedTitle);
    public string SecondaryPanelTitle => Selection.SecondaryPanelTitle;
    public IReadOnlyList<string> CenterPanelSplitModes => MiddlePanelSplitModes;

    public CatalogNode? SelectedNode
    {
        get => Selection.SelectedNode;
        set
        {
            if (!SetComponentField(Selection.SelectedNode, value, v => Selection.SelectedNode = v))
                return;
            OnPropertyChanged(nameof(SelectedTitle));
            OnPropertyChanged(nameof(MiddlePanelTitle));
            OnPropertyChanged(nameof(PrimaryPanelTitle));
            OnPropertyChanged(nameof(IsUnorganizedEditorVisible));
            OnPropertyChanged(nameof(IsStandardEditorVisible));
            OnSelectedNodeChanged();
        }
    }

    public string FilterText
    {
        get => Selection.FilterText;
        set
        {
            if (!SetComponentField(Selection.FilterText, value, v => Selection.FilterText = v))
                return;
            RefreshVisibleItems();
        }
    }

    public string MiddlePanelSplitMode
    {
        get => Selection.MiddlePanelSplitMode;
        set
        {
            var previous = Selection.MiddlePanelSplitMode;
            if (!SetComponentField(previous, value, v => Selection.MiddlePanelSplitMode = v))
                return;

            OnPropertyChanged(nameof(IsCenterPanelSingleView));
            OnPropertyChanged(nameof(IsCenterPanelSplitVertically));
            OnPropertyChanged(nameof(IsCenterPanelSplitHorizontally));
            OnMiddlePanelSplitModeChanged(previous, value);
        }
    }

    public string NewCategoryName { get => EditorInputs.NewCategoryName; set => SetComponentField(EditorInputs.NewCategoryName, value, v => EditorInputs.NewCategoryName = v); }
    public string NewCategoryRuleMode { get => EditorInputs.NewCategoryRuleMode; set => SetComponentField(EditorInputs.NewCategoryRuleMode, value, v => EditorInputs.NewCategoryRuleMode = v); }
    public string NewCategoryRuleMatch { get => EditorInputs.NewCategoryRuleMatch; set => SetComponentField(EditorInputs.NewCategoryRuleMatch, value, v => EditorInputs.NewCategoryRuleMatch = v); }
    public string NewCategoryRuleTerm { get => EditorInputs.NewCategoryRuleTerm; set => SetComponentField(EditorInputs.NewCategoryRuleTerm, value, v => EditorInputs.NewCategoryRuleTerm = v); }
    public string NewCategoryFirstOrder { get => EditorInputs.NewCategoryFirstOrder; set => SetComponentField(EditorInputs.NewCategoryFirstOrder, value, v => EditorInputs.NewCategoryFirstOrder = v); }
    public string NewCategoryLastOrder { get => EditorInputs.NewCategoryLastOrder; set => SetComponentField(EditorInputs.NewCategoryLastOrder, value, v => EditorInputs.NewCategoryLastOrder = v); }

    public string SelectedCategoryForShop
    {
        get => EditorInputs.SelectedCategoryForShop;
        set
        {
            if (!SetComponentField(EditorInputs.SelectedCategoryForShop, value, v => EditorInputs.SelectedCategoryForShop = v))
                return;
            OnPropertyChanged(nameof(PrimaryPanelTitle));
            OnSelectedCategoryChanged();
        }
    }

    public string SelectedShopName
    {
        get => EditorInputs.SelectedShopName;
        set
        {
            if (!SetComponentField(EditorInputs.SelectedShopName, value, v => EditorInputs.SelectedShopName = v))
                return;
            OnPropertyChanged(nameof(PrimaryPanelTitle));
            OnSelectedShopChanged();
        }
    }

    public string NewShopName { get => EditorInputs.NewShopName; set => SetComponentField(EditorInputs.NewShopName, value, v => EditorInputs.NewShopName = v); }
    public string NewShopPriority { get => EditorInputs.NewShopPriority; set => SetComponentField(EditorInputs.NewShopPriority, value, v => EditorInputs.NewShopPriority = v); }
    public string NewShopRuleMode { get => EditorInputs.NewShopRuleMode; set => SetComponentField(EditorInputs.NewShopRuleMode, value, v => EditorInputs.NewShopRuleMode = v); }
    public string NewShopRuleMatch { get => EditorInputs.NewShopRuleMatch; set => SetComponentField(EditorInputs.NewShopRuleMatch, value, v => EditorInputs.NewShopRuleMatch = v); }
    public string NewShopRuleTerm { get => EditorInputs.NewShopRuleTerm; set => SetComponentField(EditorInputs.NewShopRuleTerm, value, v => EditorInputs.NewShopRuleTerm = v); }
    public string NewShopFirstOrder { get => EditorInputs.NewShopFirstOrder; set => SetComponentField(EditorInputs.NewShopFirstOrder, value, v => EditorInputs.NewShopFirstOrder = v); }
    public string NewShopLastOrder { get => EditorInputs.NewShopLastOrder; set => SetComponentField(EditorInputs.NewShopLastOrder, value, v => EditorInputs.NewShopLastOrder = v); }
    public string NewUnorganizedRuleMode { get => EditorInputs.NewUnorganizedRuleMode; set => SetComponentField(EditorInputs.NewUnorganizedRuleMode, value, v => EditorInputs.NewUnorganizedRuleMode = v); }
    public string NewUnorganizedRuleMatch { get => EditorInputs.NewUnorganizedRuleMatch; set => SetComponentField(EditorInputs.NewUnorganizedRuleMatch, value, v => EditorInputs.NewUnorganizedRuleMatch = v); }
    public string NewUnorganizedRuleTerm { get => EditorInputs.NewUnorganizedRuleTerm; set => SetComponentField(EditorInputs.NewUnorganizedRuleTerm, value, v => EditorInputs.NewUnorganizedRuleTerm = v); }

    public string UnorganizedBlacklistedFilterText
    {
        get => EditorInputs.UnorganizedBlacklistedFilterText;
        set
        {
            if (!SetComponentField(EditorInputs.UnorganizedBlacklistedFilterText, value, v => EditorInputs.UnorganizedBlacklistedFilterText = v))
                return;
            RefreshFilteredBlacklistedUnorganizedItems();
        }
    }

    public int RightEditorTabIndex { get => Selection.RightEditorTabIndex; set => SetComponentField(Selection.RightEditorTabIndex, value, v => Selection.RightEditorTabIndex = v); }
    public MerchantShopBadgeRow? SelectedMerchantAddShop { get => Selection.SelectedMerchantAddShop; set => SetComponentField(Selection.SelectedMerchantAddShop, value, v => Selection.SelectedMerchantAddShop = v); }

    public MerchantTreeNode? SelectedMerchantWorkspaceNode
    {
        get => Selection.SelectedMerchantWorkspaceNode;
        set
        {
            if (ReferenceEquals(Selection.SelectedMerchantWorkspaceNode, value))
                return;

            Selection.SelectedMerchantWorkspaceNode = value;
            OnPropertyChanged();
            var nextMerchant = value?.Title ?? string.Empty;
            if (!string.Equals(Selection.SelectedMerchantName, nextMerchant, StringComparison.OrdinalIgnoreCase))
                SelectedMerchantName = nextMerchant;
        }
    }

    public string SelectedMerchantName
    {
        get => Selection.SelectedMerchantName;
        set
        {
            if (!SetComponentField(Selection.SelectedMerchantName, value, v => Selection.SelectedMerchantName = v))
                return;
            OnPropertyChanged(nameof(SelectedMerchantDisplayName));
            OnPropertyChanged(nameof(HasSelectedMerchant));
            OnPropertyChanged(nameof(MiddlePanelTitle));
            OnPropertyChanged(nameof(IsUnorganizedEditorVisible));
            OnPropertyChanged(nameof(IsStandardEditorVisible));
            OnSelectedMerchantChanged();
        }
    }

    public string SelectedMerchantDisplayName => string.IsNullOrWhiteSpace(Selection.SelectedMerchantName) ? "No merchant selected" : Selection.SelectedMerchantName;
    public bool HasSelectedMerchant => !string.IsNullOrWhiteSpace(Selection.SelectedMerchantName);
    public bool IsCatalogEditorVisible => true;
    public bool IsMerchantEditorVisible => false;
    public bool IsCenterPanelSingleView => string.Equals(Selection.MiddlePanelSplitMode, "Single View", StringComparison.OrdinalIgnoreCase);
    public bool IsCenterPanelSplitVertically => string.Equals(Selection.MiddlePanelSplitMode, "Split Vertically", StringComparison.OrdinalIgnoreCase);
    public bool IsCenterPanelSplitHorizontally => string.Equals(Selection.MiddlePanelSplitMode, "Split Horizontally", StringComparison.OrdinalIgnoreCase);
    public bool IsUnorganizedEditorVisible => IsCatalogEditorVisible && Selection.SelectedNode?.Type == CatalogNodeType.Group;
    public bool IsStandardEditorVisible => !IsUnorganizedEditorVisible;

    public int BlacklistedUnorganizedCount => BlacklistedUnorganizedItems.Count;

    protected void SetSecondaryPanelTitle(string title)
    {
        if (!SetComponentField(Selection.SecondaryPanelTitle, title, v => Selection.SecondaryPanelTitle = v))
            return;

        OnPropertyChanged(nameof(SecondaryPanelTitle));
    }

    protected string GetCurrentShopViewTitle()
    {
        return BuildShopViewTitle(SelectedCategoryForShop, SelectedShopName, SelectedTitle);
    }

    protected virtual void OnMiddlePanelSplitModeChanged(string previousMode, string nextMode)
    {
    }

    private static string BuildShopViewTitle(string categoryName, string shopName, string fallback)
    {
        if (!string.IsNullOrWhiteSpace(categoryName) && !string.IsNullOrWhiteSpace(shopName))
            return $"{categoryName} / {shopName}";

        return fallback;
    }

    protected abstract void RefreshFilteredBlacklistedUnorganizedItems();
}
