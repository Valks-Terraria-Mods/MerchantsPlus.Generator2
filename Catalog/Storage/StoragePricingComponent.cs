using System.Text.Json;

namespace MerchantsPlus.Generator;

internal static class StoragePricingComponent
{
    internal static GlobalFlagsConfig LoadGlobalFlagsConfig(string pricingDirectory, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "global-flags.json");

        if (!File.Exists(path))
            return new GlobalFlagsConfig();

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<GlobalFlagsConfig>(json, options);
        return data ?? new GlobalFlagsConfig();
    }

    internal static void SaveGlobalFlagsConfig(string pricingDirectory, GlobalFlagsConfig config, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "global-flags.json");

        var normalizedRules = KeywordRuleSet.Normalize(config.Rules.Length > 0
            ? config.Rules
            : KeywordRuleSet.FromLegacyUnorganized(new UnorganizedBlacklistConfig
            {
                WhitelistKeywords = config.WhitelistKeywords,
                WhitelistPrefixKeywords = config.WhitelistPrefixKeywords,
                WhitelistSuffixKeywords = config.WhitelistSuffixKeywords,
                PrefixKeywords = config.PrefixKeywords,
                SuffixKeywords = config.SuffixKeywords,
                ContainsKeywords = config.ContainsKeywords
            }));

        var stable = new GlobalFlagsConfig
        {
            Rules = normalizedRules,
            WhitelistKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistPrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistSuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.WhitelistMode),
            PrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.BlacklistMode),
            SuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.BlacklistMode),
            ContainsKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.BlacklistMode),
            FirstOrder = config.FirstOrder,
            LastOrder = config.LastOrder
        };

        WriteIndentedJson(path, stable, options);
    }

    internal static UnorganizedBlacklistConfig LoadUnorganizedBlacklistConfig(string pricingDirectory, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "unorganized-blacklist.json");

        if (!File.Exists(path))
            return new UnorganizedBlacklistConfig();

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<UnorganizedBlacklistConfig>(json, options);
        return data ?? new UnorganizedBlacklistConfig();
    }

    internal static void SaveUnorganizedBlacklistConfig(string pricingDirectory, UnorganizedBlacklistConfig config, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "unorganized-blacklist.json");

        var normalizedRules = KeywordRuleSet.Normalize(config.Rules.Length > 0
            ? config.Rules
            : KeywordRuleSet.FromLegacyUnorganized(config));

        var stable = new UnorganizedBlacklistConfig
        {
            Rules = normalizedRules,
            WhitelistKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistPrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.WhitelistMode),
            WhitelistSuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.WhitelistMode),
            PrefixKeywords = KeywordRuleSet.ToLegacyPrefix(normalizedRules, KeywordRule.BlacklistMode),
            SuffixKeywords = KeywordRuleSet.ToLegacySuffix(normalizedRules, KeywordRule.BlacklistMode),
            ContainsKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.BlacklistMode)
        };

        WriteIndentedJson(path, stable, options);
    }

    internal static Dictionary<string, int> LoadItemPriceOverrides(string pricingDirectory, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "item-price-overrides.json");

        if (!File.Exists(path))
            return new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<Dictionary<string, int>>(json, options);
        return data ?? new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    }

    internal static void SaveItemPriceOverrides(string pricingDirectory, IReadOnlyDictionary<string, int> overrides, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "item-price-overrides.json");

        var stable = overrides
            .OrderBy(v => v.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(v => v.Key, v => v.Value, StringComparer.OrdinalIgnoreCase);

        WriteIndentedJson(path, stable, options);
    }

    internal static int LoadUnassignedDefaultPrice(string pricingDirectory, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "unassigned-item-price.json");

        if (!File.Exists(path))
            return 0;

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<int?>(json, options);
        return data is null || data <= 0 ? 0 : data.Value;
    }

    internal static void SaveUnassignedDefaultPrice(string pricingDirectory, int copper, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "unassigned-item-price.json");
        WriteIndentedJson(path, Math.Max(0, copper), options);
    }

    internal static Dictionary<string, string[]> LoadItemConditions(string pricingDirectory, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "item-conditions.json");

        if (!File.Exists(path))
            return new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        var json = File.ReadAllText(path);
        var data = JsonSerializer.Deserialize<Dictionary<string, string[]>>(json, options);
        return data ?? new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
    }

    internal static void SaveItemConditions(string pricingDirectory, IReadOnlyDictionary<string, string[]> itemConditions, JsonSerializerOptions options)
    {
        var path = GetPricingFilePath(pricingDirectory, "item-conditions.json");

        var stable = itemConditions
            .OrderBy(v => v.Key, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(
                v => v.Key,
                v => v.Value
                    .Where(c => !string.IsNullOrWhiteSpace(c))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
                    .ToArray(),
                StringComparer.OrdinalIgnoreCase);

        WriteIndentedJson(path, stable, options);
    }

    private static string GetPricingFilePath(string pricingDirectory, string fileName)
    {
        Directory.CreateDirectory(pricingDirectory);
        return Path.Combine(pricingDirectory, fileName);
    }

    private static void WriteIndentedJson<T>(string path, T data, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Serialize(data, options);
        json = StoragePathHelper.ConvertToFourSpaceIndent(json);
        File.WriteAllText(path, json);
    }
}
