namespace MerchantsPlus.Generator;

public sealed class MerchantWorkspaceBuildResult{
    private readonly IReadOnlyList<MerchantTreeNode> _workspaceNodes;
    private readonly string _selectedMerchantName;

    public MerchantWorkspaceBuildResult(IReadOnlyList<MerchantTreeNode> workspaceNodes, string selectedMerchantName)
    {
        _workspaceNodes = workspaceNodes;
        _selectedMerchantName = selectedMerchantName;
    }

    public IReadOnlyList<MerchantTreeNode> WorkspaceNodes => _workspaceNodes;
    public string SelectedMerchantName => _selectedMerchantName;
}
