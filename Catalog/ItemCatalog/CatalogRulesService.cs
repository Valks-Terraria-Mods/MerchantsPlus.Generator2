namespace MerchantsPlus.Generator;

public class CatalogRulesService{
    private readonly ItemCatalogState _state;
    private readonly CatalogStorageGateway _storage;

    public CatalogRulesService(ItemCatalogState state, CatalogStorageGateway storage)
    {
        _state = state;
        _storage = storage;
    }

    public GlobalFlagsConfig GetGlobalFlagsConfig()
    {
        var source = _state.GlobalFlagsConfig;
        var normalizedRules = KeywordRuleSet.Normalize(source.Rules.Length > 0
            ? source.Rules
            : KeywordRuleSet.FromLegacyUnorganized(new UnorganizedBlacklistConfig
            {
                WhitelistKeywords = source.WhitelistKeywords,
                WhitelistPrefixKeywords = source.WhitelistPrefixKeywords,
                WhitelistSuffixKeywords = source.WhitelistSuffixKeywords,
                PrefixKeywords = source.PrefixKeywords,
                SuffixKeywords = source.SuffixKeywords,
                ContainsKeywords = source.ContainsKeywords
            }));

        return new GlobalFlagsConfig
        {
            Rules = normalizedRules,
            WhitelistKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistPrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistSuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.WhitelistMode),
            PrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.BlacklistMode),
            SuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.BlacklistMode),
            ContainsKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.BlacklistMode),
            FirstOrder = [.. source.FirstOrder],
            LastOrder = [.. source.LastOrder]
        };
    }

    public void UpdateGlobalFlagsConfig(IEnumerable<KeywordRule> rules, string[] firstOrder, string[] lastOrder)
    {
        var normalizedRules = KeywordRuleSet.Normalize(rules);

        var updatedConfig = new GlobalFlagsConfig
        {
            Rules = normalizedRules,
            WhitelistKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistPrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistSuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.WhitelistMode),
            PrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.BlacklistMode),
            SuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.BlacklistMode),
            ContainsKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.BlacklistMode),
            FirstOrder = firstOrder,
            LastOrder = lastOrder
        };

        _state.SetGlobalFlagsConfig(updatedConfig);
        _storage.SaveGlobalFlagsConfig(updatedConfig);
    }

    public UnorganizedBlacklistConfig GetUnorganizedBlacklistConfig()
    {
        var source = _state.UnorganizedBlacklistConfig;
        var normalizedRules = KeywordRuleSet.Normalize(source.Rules.Length > 0
            ? source.Rules
            : KeywordRuleSet.FromLegacyUnorganized(source));

        return new UnorganizedBlacklistConfig
        {
            Rules = normalizedRules,
            WhitelistKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistPrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistSuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.WhitelistMode),
            PrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.BlacklistMode),
            SuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.BlacklistMode),
            ContainsKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.BlacklistMode)
        };
    }

    public void UpdateUnorganizedBlacklistConfig(IEnumerable<KeywordRule> rules)
    {
        var normalizedRules = KeywordRuleSet.Normalize(rules);

        var updatedConfig = new UnorganizedBlacklistConfig
        {
            Rules = normalizedRules,
            WhitelistKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistPrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistSuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.WhitelistMode),
            PrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.BlacklistMode),
            SuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.BlacklistMode),
            ContainsKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.BlacklistMode)
        };

        _state.SetUnorganizedBlacklistConfig(updatedConfig);
        _storage.SaveUnorganizedBlacklistConfig(updatedConfig);
    }
}