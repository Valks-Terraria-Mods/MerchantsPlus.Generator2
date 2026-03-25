using System.Collections.ObjectModel;

namespace MerchantsPlus.Generator;

public sealed class CatalogCollectionsComponent{
    private readonly ObservableCollection<CatalogNode> _rootNodes = [];
    private readonly ObservableCollection<CatalogItemRow> _visibleItems = [];
    private readonly ObservableCollection<CatalogItemRow> _secondaryVisibleItems = [];
    private readonly ObservableCollection<MerchantTreeNode> _visibleMerchantAssignments = [];
    private readonly ObservableCollection<MerchantTreeNode> _merchantWorkspaceNodes = [];
    private readonly ObservableCollection<MerchantShopBadgeRow> _merchantOwnedShopBadges = [];
    private readonly ObservableCollection<MerchantShopBadgeRow> _merchantAvailableShops = [];
    private readonly ObservableCollection<string> _merchantNpcIds = [];
    private readonly ObservableCollection<string> _outputFiles = [];
    private readonly ObservableCollection<AssignmentTreeNode> _merchantAssignmentOptionsTree = [];
    private readonly ObservableCollection<AssignmentTreeNode> _merchantAssignmentAvailableTree = [];
    private readonly ObservableCollection<AssignmentTreeNode> _merchantAssignmentAssignedTree = [];
    private readonly ObservableCollection<SelectableOption> _merchantCategoryOptions = [];
    private readonly ObservableCollection<SelectableOption> _merchantShopOptions = [];
    private readonly ObservableCollection<string> _categoryOptions = [];
    private readonly ObservableCollection<string> _shopOptions = [];
    private readonly ObservableCollection<KeywordRuleEditorRow> _categoryKeywordRules = [];
    private readonly ObservableCollection<KeywordRuleEditorRow> _shopKeywordRules = [];
    private readonly ObservableCollection<KeywordRuleEditorRow> _unorganizedKeywordRules = [];
    private readonly ObservableCollection<string> _blacklistedUnorganizedItems = [];
    private readonly ObservableCollection<string> _filteredBlacklistedUnorganizedItems = [];
    private readonly IReadOnlyList<string> _ruleModes = [KeywordRule.WhitelistMode, KeywordRule.BlacklistMode];
    private readonly IReadOnlyList<string> _ruleMatches = [KeywordRule.ContainsMatch, KeywordRule.PrefixMatch, KeywordRule.SuffixMatch, KeywordRule.ExactMatch];
    private readonly IReadOnlyList<string> _merchantSortModes = ["Name", "Item Count", "Shop Count"];

    public ObservableCollection<CatalogNode> RootNodes => _rootNodes;
    public ObservableCollection<CatalogItemRow> VisibleItems => _visibleItems;
    public ObservableCollection<CatalogItemRow> SecondaryVisibleItems => _secondaryVisibleItems;
    public ObservableCollection<MerchantTreeNode> VisibleMerchantAssignments => _visibleMerchantAssignments;
    public ObservableCollection<MerchantTreeNode> MerchantWorkspaceNodes => _merchantWorkspaceNodes;
    public ObservableCollection<MerchantShopBadgeRow> MerchantOwnedShopBadges => _merchantOwnedShopBadges;
    public ObservableCollection<MerchantShopBadgeRow> MerchantAvailableShops => _merchantAvailableShops;
    public ObservableCollection<string> MerchantNpcIds => _merchantNpcIds;
    public ObservableCollection<string> OutputFiles => _outputFiles;
    public ObservableCollection<AssignmentTreeNode> MerchantAssignmentOptionsTree => _merchantAssignmentOptionsTree;
    public ObservableCollection<AssignmentTreeNode> MerchantAssignmentAvailableTree => _merchantAssignmentAvailableTree;
    public ObservableCollection<AssignmentTreeNode> MerchantAssignmentAssignedTree => _merchantAssignmentAssignedTree;
    public ObservableCollection<SelectableOption> MerchantCategoryOptions => _merchantCategoryOptions;
    public ObservableCollection<SelectableOption> MerchantShopOptions => _merchantShopOptions;
    public ObservableCollection<string> CategoryOptions => _categoryOptions;
    public ObservableCollection<string> ShopOptions => _shopOptions;
    public ObservableCollection<KeywordRuleEditorRow> CategoryKeywordRules => _categoryKeywordRules;
    public ObservableCollection<KeywordRuleEditorRow> ShopKeywordRules => _shopKeywordRules;
    public ObservableCollection<KeywordRuleEditorRow> UnorganizedKeywordRules => _unorganizedKeywordRules;
    public ObservableCollection<string> BlacklistedUnorganizedItems => _blacklistedUnorganizedItems;
    public ObservableCollection<string> FilteredBlacklistedUnorganizedItems => _filteredBlacklistedUnorganizedItems;
    public IReadOnlyList<string> RuleModes => _ruleModes;
    public IReadOnlyList<string> RuleMatches => _ruleMatches;
    public IReadOnlyList<string> MerchantSortModes => _merchantSortModes;
}
