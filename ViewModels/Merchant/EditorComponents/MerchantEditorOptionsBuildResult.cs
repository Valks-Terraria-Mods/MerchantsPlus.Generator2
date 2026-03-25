namespace MerchantsPlus.Generator;

public sealed class MerchantEditorOptionsBuildResult{
    private readonly IReadOnlyList<SelectableOption> _categoryOptions;
    private readonly IReadOnlyList<SelectableOption> _shopOptions;
    private readonly IReadOnlyList<AssignmentTreeNode> _optionsTree;
    private readonly IReadOnlyList<AssignmentTreeNode> _availableTree;
    private readonly IReadOnlyList<AssignmentTreeNode> _assignedTree;
    private readonly string _assignmentHint;

    public MerchantEditorOptionsBuildResult(
        IReadOnlyList<SelectableOption> categoryOptions,
        IReadOnlyList<SelectableOption> shopOptions,
        IReadOnlyList<AssignmentTreeNode> optionsTree,
        IReadOnlyList<AssignmentTreeNode> availableTree,
        IReadOnlyList<AssignmentTreeNode> assignedTree,
        string assignmentHint)
    {
        _categoryOptions = categoryOptions;
        _shopOptions = shopOptions;
        _optionsTree = optionsTree;
        _availableTree = availableTree;
        _assignedTree = assignedTree;
        _assignmentHint = assignmentHint;
    }

    public IReadOnlyList<SelectableOption> CategoryOptions => _categoryOptions;
    public IReadOnlyList<SelectableOption> ShopOptions => _shopOptions;
    public IReadOnlyList<AssignmentTreeNode> OptionsTree => _optionsTree;
    public IReadOnlyList<AssignmentTreeNode> AvailableTree => _availableTree;
    public IReadOnlyList<AssignmentTreeNode> AssignedTree => _assignedTree;
    public string AssignmentHint => _assignmentHint;
}
