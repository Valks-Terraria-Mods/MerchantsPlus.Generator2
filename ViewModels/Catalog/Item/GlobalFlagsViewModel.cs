using System.Collections.ObjectModel;

namespace MerchantsPlus.Generator;

public abstract class GlobalFlagsViewModel : PricingEditorViewModel
{
    private string _globalFirstOrderItems = string.Empty;
    private string _globalLastOrderItems = string.Empty;
    private string _newGlobalWhitelistRuleMatch = KeywordRule.ContainsMatch;
    private string _newGlobalWhitelistRuleTerm = string.Empty;
    private string _newGlobalBlacklistRuleMatch = KeywordRule.ContainsMatch;
    private string _newGlobalBlacklistRuleTerm = string.Empty;

    public ObservableCollection<KeywordRuleEditorRow> GlobalWhitelistRules { get; } = [];
    public ObservableCollection<KeywordRuleEditorRow> GlobalBlacklistRules { get; } = [];
    public string GlobalFirstOrderItems { get => _globalFirstOrderItems; set => SetField(ref _globalFirstOrderItems, value); }
    public string GlobalLastOrderItems { get => _globalLastOrderItems; set => SetField(ref _globalLastOrderItems, value); }
    public string NewGlobalWhitelistRuleMatch { get => _newGlobalWhitelistRuleMatch; set => SetField(ref _newGlobalWhitelistRuleMatch, value); }
    public string NewGlobalWhitelistRuleTerm { get => _newGlobalWhitelistRuleTerm; set => SetField(ref _newGlobalWhitelistRuleTerm, value); }
    public string NewGlobalBlacklistRuleMatch { get => _newGlobalBlacklistRuleMatch; set => SetField(ref _newGlobalBlacklistRuleMatch, value); }
    public string NewGlobalBlacklistRuleTerm { get => _newGlobalBlacklistRuleTerm; set => SetField(ref _newGlobalBlacklistRuleTerm, value); }

    public void LoadGlobalFlagsSettings()
    {
        var config = ItemCatalog.GetGlobalFlagsConfig();
        var normalizedRules = KeywordRuleSet.Normalize(config.Rules);

        GlobalFirstOrderItems = string.Join(", ", config.FirstOrder);
        GlobalLastOrderItems = string.Join(", ", config.LastOrder);
        LoadRuleRows(GlobalWhitelistRules, normalizedRules.Where(rule => string.Equals(rule.Mode, KeywordRule.WhitelistMode, StringComparison.OrdinalIgnoreCase)));
        LoadRuleRows(GlobalBlacklistRules, normalizedRules.Where(rule => string.Equals(rule.Mode, KeywordRule.BlacklistMode, StringComparison.OrdinalIgnoreCase)));
        NewGlobalWhitelistRuleMatch = KeywordRule.ContainsMatch;
        NewGlobalWhitelistRuleTerm = string.Empty;
        NewGlobalBlacklistRuleMatch = KeywordRule.ContainsMatch;
        NewGlobalBlacklistRuleTerm = string.Empty;
        LoadUnassignedDefaultPrice();
    }

    public void AddGlobalWhitelistRule()
    {
        AddGlobalRule(GlobalWhitelistRules, KeywordRule.WhitelistMode, NewGlobalWhitelistRuleMatch, NewGlobalWhitelistRuleTerm, value => NewGlobalWhitelistRuleTerm = value);
    }

    public void RemoveGlobalWhitelistRule(KeywordRuleEditorRow row)
    {
        GlobalWhitelistRules.Remove(row);
    }

    public void AddGlobalBlacklistRule()
    {
        AddGlobalRule(GlobalBlacklistRules, KeywordRule.BlacklistMode, NewGlobalBlacklistRuleMatch, NewGlobalBlacklistRuleTerm, value => NewGlobalBlacklistRuleTerm = value);
    }

    public void RemoveGlobalBlacklistRule(KeywordRuleEditorRow row)
    {
        GlobalBlacklistRules.Remove(row);
    }

    public void ApplyGlobalFlags()
    {
        var globalFirstOrder = ParseCommaSeparated(GlobalFirstOrderItems);
        var globalLastOrder = ParseCommaSeparated(GlobalLastOrderItems);
        var globalRules = KeywordRuleSet.Normalize(
        [
            .. GlobalWhitelistRules.Select(row => row.ToRule()),
            .. GlobalBlacklistRules.Select(row => row.ToRule())
        ]);

        ItemCatalog.UpdateGlobalFlagsConfig(globalRules, globalFirstOrder, globalLastOrder);
        AutoRebuildCatalog();
        EditorStatus = "Applied global flags to all shops with global rule priority 0 (Unorganized excluded).";
    }

    public void ResetAllPrices()
    {
        var allShopItems = ItemCatalog.Categories
            .SelectMany(category => category.Shops)
            .SelectMany(shop => shop.Items)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var resetCount = ItemPrices.ClearOverrides(allShopItems);
        ItemPrices.SetUnassignedDefaultPriceCopper(0);
        LoadUnassignedDefaultPrice();
        RefreshVisibleItemPrices();
        LoadSelectedItemPrice();
        SaveMerchantAssignmentsAndOutput();
        EditorStatus = $"Reset all price overrides across shops ({resetCount} items) and cleared unassigned default price.";
    }

    private static void AddGlobalRule(ObservableCollection<KeywordRuleEditorRow> target, string mode, string match, string term, Action<string> clearTerm)
    {
        if (string.IsNullOrWhiteSpace(term))
            return;

        var candidate = new KeywordRuleEditorRow
        {
            Mode = mode,
            Match = match,
            Term = term.Trim()
        };

        var exists = target.Any(row =>
            string.Equals(row.Mode, candidate.Mode, StringComparison.OrdinalIgnoreCase)
            && string.Equals(row.Match, candidate.Match, StringComparison.OrdinalIgnoreCase)
            && string.Equals(row.Term, candidate.Term, StringComparison.OrdinalIgnoreCase));

        if (!exists)
            target.Add(candidate);

        clearTerm(string.Empty);
    }
}