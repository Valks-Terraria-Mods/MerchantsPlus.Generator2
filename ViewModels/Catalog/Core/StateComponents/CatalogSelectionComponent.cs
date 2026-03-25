namespace MerchantsPlus.Generator;

public sealed class CatalogSelectionComponent{
    private CatalogNode? _selectedNode;
    private string _filterText = string.Empty;
    private int _workspaceTabIndex;
    private int _rightEditorTabIndex;
    private string _selectedMerchantName = string.Empty;
    private MerchantTreeNode? _selectedMerchantWorkspaceNode;
    private MerchantShopBadgeRow? _selectedMerchantAddShop;
    private string _merchantAssignmentHint = "Assign categories and shops to the selected merchant.";
    private string _editorStatus = "Configure categories and shops, then rebuild.";
    private string _merchantSortMode = "Name";
    private string _middlePanelSplitMode = "Single View";
    private string _secondaryPanelTitle = string.Empty;

    public CatalogNode? SelectedNode { get => _selectedNode; set => _selectedNode = value; }
    public string FilterText { get => _filterText; set => _filterText = value; }
    public int WorkspaceTabIndex { get => _workspaceTabIndex; set => _workspaceTabIndex = value; }
    public int RightEditorTabIndex { get => _rightEditorTabIndex; set => _rightEditorTabIndex = value; }
    public string SelectedMerchantName { get => _selectedMerchantName; set => _selectedMerchantName = value; }
    public MerchantTreeNode? SelectedMerchantWorkspaceNode { get => _selectedMerchantWorkspaceNode; set => _selectedMerchantWorkspaceNode = value; }
    public MerchantShopBadgeRow? SelectedMerchantAddShop { get => _selectedMerchantAddShop; set => _selectedMerchantAddShop = value; }
    public string MerchantAssignmentHint { get => _merchantAssignmentHint; set => _merchantAssignmentHint = value; }
    public string EditorStatus { get => _editorStatus; set => _editorStatus = value; }
    public string MerchantSortMode { get => _merchantSortMode; set => _merchantSortMode = value; }
    public string MiddlePanelSplitMode { get => _middlePanelSplitMode; set => _middlePanelSplitMode = value; }
    public string SecondaryPanelTitle { get => _secondaryPanelTitle; set => _secondaryPanelTitle = value; }
}
