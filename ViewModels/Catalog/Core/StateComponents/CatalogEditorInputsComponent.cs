namespace MerchantsPlus.Generator;

public sealed class CatalogEditorInputsComponent{
    private string _newCategoryName = string.Empty;
    private string _newCategoryRuleMode = KeywordRule.WhitelistMode;
    private string _newCategoryRuleMatch = KeywordRule.ContainsMatch;
    private string _newCategoryRuleTerm = string.Empty;
    private string _newCategoryFirstOrder = string.Empty;
    private string _newCategoryLastOrder = string.Empty;
    private string _selectedCategoryForShop = string.Empty;
    private string _newShopName = string.Empty;
    private string _newShopPriority = "0";
    private string _newShopRuleMode = KeywordRule.WhitelistMode;
    private string _newShopRuleMatch = KeywordRule.ContainsMatch;
    private string _newShopRuleTerm = string.Empty;
    private string _newShopFirstOrder = string.Empty;
    private string _newShopLastOrder = string.Empty;
    private string _newUnorganizedRuleMode = KeywordRule.BlacklistMode;
    private string _newUnorganizedRuleMatch = KeywordRule.ContainsMatch;
    private string _newUnorganizedRuleTerm = string.Empty;
    private string _unorganizedBlacklistedFilterText = string.Empty;
    private string _selectedShopName = string.Empty;

    public string NewCategoryName { get => _newCategoryName; set => _newCategoryName = value; }
    public string NewCategoryRuleMode { get => _newCategoryRuleMode; set => _newCategoryRuleMode = value; }
    public string NewCategoryRuleMatch { get => _newCategoryRuleMatch; set => _newCategoryRuleMatch = value; }
    public string NewCategoryRuleTerm { get => _newCategoryRuleTerm; set => _newCategoryRuleTerm = value; }
    public string NewCategoryFirstOrder { get => _newCategoryFirstOrder; set => _newCategoryFirstOrder = value; }
    public string NewCategoryLastOrder { get => _newCategoryLastOrder; set => _newCategoryLastOrder = value; }
    public string SelectedCategoryForShop { get => _selectedCategoryForShop; set => _selectedCategoryForShop = value; }
    public string NewShopName { get => _newShopName; set => _newShopName = value; }
    public string NewShopPriority { get => _newShopPriority; set => _newShopPriority = value; }
    public string NewShopRuleMode { get => _newShopRuleMode; set => _newShopRuleMode = value; }
    public string NewShopRuleMatch { get => _newShopRuleMatch; set => _newShopRuleMatch = value; }
    public string NewShopRuleTerm { get => _newShopRuleTerm; set => _newShopRuleTerm = value; }
    public string NewShopFirstOrder { get => _newShopFirstOrder; set => _newShopFirstOrder = value; }
    public string NewShopLastOrder { get => _newShopLastOrder; set => _newShopLastOrder = value; }
    public string NewUnorganizedRuleMode { get => _newUnorganizedRuleMode; set => _newUnorganizedRuleMode = value; }
    public string NewUnorganizedRuleMatch { get => _newUnorganizedRuleMatch; set => _newUnorganizedRuleMatch = value; }
    public string NewUnorganizedRuleTerm { get => _newUnorganizedRuleTerm; set => _newUnorganizedRuleTerm = value; }
    public string UnorganizedBlacklistedFilterText { get => _unorganizedBlacklistedFilterText; set => _unorganizedBlacklistedFilterText = value; }
    public string SelectedShopName { get => _selectedShopName; set => _selectedShopName = value; }
}
