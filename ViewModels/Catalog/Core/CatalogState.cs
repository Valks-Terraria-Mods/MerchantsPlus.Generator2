using System.Collections.ObjectModel;

namespace MerchantsPlus.Generator;

public abstract class CatalogState : CatalogStateEditorView
{
    protected CatalogState() { }

    protected CatalogState(CatalogStateComponents components) : base(components) { }

    protected static string[] ParseCommaSeparated(string input)
    {
        return input.Split([',', '\n', '\r', ';'], StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
    }

    protected static string FormatExcludedItems(IEnumerable<string> items)
    {
        var ordered = items
            .Select(v => v.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v, StringComparer.OrdinalIgnoreCase);

        return string.Join(", ", ordered);
    }

    protected void ClearShopInputs()
    {
        NewShopName = string.Empty;
        NewShopPriority = "0";
        ShopKeywordRules.Clear();
        NewShopRuleMode = KeywordRule.WhitelistMode;
        NewShopRuleMatch = KeywordRule.ContainsMatch;
        NewShopRuleTerm = string.Empty;
        NewShopFirstOrder = string.Empty;
        NewShopLastOrder = string.Empty;
    }

    protected void LoadUnorganizedBlacklistInputs()
    {
        var config = ItemCatalog.GetUnorganizedBlacklistConfig();
        LoadRuleRows(UnorganizedKeywordRules, config.Rules.Length > 0 ? config.Rules : KeywordRuleSet.FromLegacyUnorganized(config));
        NewUnorganizedRuleMode = KeywordRule.BlacklistMode;
        NewUnorganizedRuleMatch = KeywordRule.ContainsMatch;
        NewUnorganizedRuleTerm = string.Empty;
        RefreshBlacklistedUnorganizedItems();
    }

    protected static void LoadRuleRows(ObservableCollection<KeywordRuleEditorRow> target, IEnumerable<KeywordRule> rules)
    {
        target.Clear();

        foreach (var rule in KeywordRuleSet.Normalize(rules))
            target.Add(KeywordRuleEditorRow.FromRule(rule));
    }

    protected static KeywordRule[] CollectRules(ObservableCollection<KeywordRuleEditorRow> rows)
    {
        return KeywordRuleSet.Normalize(rows.Select(v => v.ToRule()));
    }

    protected void RefreshBlacklistedUnorganizedItems()
    {
        BlacklistedUnorganizedItems.Clear();
        foreach (var item in ItemCatalog.BlacklistedUnorganized)
            BlacklistedUnorganizedItems.Add(item);

        RefreshFilteredBlacklistedUnorganizedItems();
    }

    protected override void RefreshFilteredBlacklistedUnorganizedItems()
    {
        FilteredBlacklistedUnorganizedItems.Clear();

        foreach (var item in BlacklistedUnorganizedItems)
        {
            if (string.IsNullOrWhiteSpace(EditorInputs.UnorganizedBlacklistedFilterText) || item.Contains(EditorInputs.UnorganizedBlacklistedFilterText, StringComparison.OrdinalIgnoreCase))
                FilteredBlacklistedUnorganizedItems.Add(item);
        }

        OnPropertyChanged(nameof(BlacklistedUnorganizedCount));
    }

    protected override void UpdateTModLoaderDllPathStatus()
    {
        var trimmed = (Settings.TModLoaderDllPath ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            TModLoaderDllPathStatus = "No path set.";
            IsTModLoaderDllPathValid = false;
            return;
        }

        if (File.Exists(trimmed))
        {
            TModLoaderDllPathStatus = "File found.";
            IsTModLoaderDllPathValid = true;
            return;
        }

        TModLoaderDllPathStatus = "File not found at this path.";
        IsTModLoaderDllPathValid = false;
    }

    protected SettingsConfig BuildSettingsConfig()
    {
        return new SettingsConfig
        {
            TModLoaderDllPath = (TModLoaderDllPath ?? string.Empty).Trim(),
            AllowedMerchantNpcIds = ParseCommaSeparated(AllowedMerchantNpcIdsText),
            AlwaysIncludeMerchantNpcIds = ParseCommaSeparated(AlwaysIncludeMerchantNpcIdsText),
            DownedFlagPrefixes = ParseCommaSeparated(DownedFlagPrefixesText),
            MainFlags = ParseCommaSeparated(MainFlagsText),
            AppTextScale = AppTextScale,
            TreeViewTextScale = TreeViewTextScale,
            AppTheme = AppTheme
        };
    }
}
