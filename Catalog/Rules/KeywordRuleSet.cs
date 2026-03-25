namespace MerchantsPlus.Generator;

public static class KeywordRuleSet
{
    public static KeywordRule[] Normalize(IEnumerable<KeywordRule> rules)
    {
        return rules
            .Where(v => !string.IsNullOrWhiteSpace(v.Term))
            .Select(v => new KeywordRule
            {
                Mode = NormalizeMode(v.Mode),
                Match = NormalizeMatch(v.Match),
                Term = v.Term.Trim()
            })
            .Distinct(KeywordRuleComparer.Instance)
            .OrderBy(v => v.Mode, StringComparer.OrdinalIgnoreCase)
            .ThenBy(v => v.Match, StringComparer.OrdinalIgnoreCase)
            .ThenBy(v => v.Term, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static KeywordRule[] FromLegacyCategory(string[] keywords, string[] blacklistedKeywords)
    {
        return
        [
            .. keywords.Select(v => new KeywordRule { Mode = KeywordRule.WhitelistMode, Match = KeywordRule.ContainsMatch, Term = v }),
            .. blacklistedKeywords.Select(v => new KeywordRule { Mode = KeywordRule.BlacklistMode, Match = KeywordRule.ContainsMatch, Term = v })
        ];
    }

    public static KeywordRule[] FromLegacyShop(string[] keywords, string[] excludedItems)
    {
        return
        [
            .. keywords.Select(v => new KeywordRule { Mode = KeywordRule.WhitelistMode, Match = KeywordRule.ContainsMatch, Term = v }),
            .. excludedItems.Select(v => new KeywordRule { Mode = KeywordRule.BlacklistMode, Match = KeywordRule.ContainsMatch, Term = v })
        ];
    }

    public static KeywordRule[] FromLegacyUnorganized(UnorganizedBlacklistConfig config)
    {
        return
        [
            .. config.WhitelistKeywords.Select(v => new KeywordRule { Mode = KeywordRule.WhitelistMode, Match = KeywordRule.ContainsMatch, Term = v }),
            .. config.WhitelistPrefixKeywords.Select(v => new KeywordRule { Mode = KeywordRule.WhitelistMode, Match = KeywordRule.PrefixMatch, Term = v }),
            .. config.WhitelistSuffixKeywords.Select(v => new KeywordRule { Mode = KeywordRule.WhitelistMode, Match = KeywordRule.SuffixMatch, Term = v }),
            .. config.PrefixKeywords.Select(v => new KeywordRule { Mode = KeywordRule.BlacklistMode, Match = KeywordRule.PrefixMatch, Term = v }),
            .. config.SuffixKeywords.Select(v => new KeywordRule { Mode = KeywordRule.BlacklistMode, Match = KeywordRule.SuffixMatch, Term = v }),
            .. config.ContainsKeywords.Select(v => new KeywordRule { Mode = KeywordRule.BlacklistMode, Match = KeywordRule.ContainsMatch, Term = v })
        ];
    }

    public static string[] ToLegacyContains(IEnumerable<KeywordRule> rules, string mode)
    {
        return rules
            .Where(v => IsMode(v.Mode, mode) && IsMatch(v.Match, KeywordRule.ContainsMatch))
            .Select(v => v.Term)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static string[] ToLegacyPrefix(IEnumerable<KeywordRule> rules, string mode)
    {
        return rules
            .Where(v => IsMode(v.Mode, mode) && IsMatch(v.Match, KeywordRule.PrefixMatch))
            .Select(v => v.Term)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static string[] ToLegacySuffix(IEnumerable<KeywordRule> rules, string mode)
    {
        return rules
            .Where(v => IsMode(v.Mode, mode) && IsMatch(v.Match, KeywordRule.SuffixMatch))
            .Select(v => v.Term)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static string NormalizeMode(string mode)
    {
        return IsMode(mode, KeywordRule.BlacklistMode) ? KeywordRule.BlacklistMode : KeywordRule.WhitelistMode;
    }

    private static string NormalizeMatch(string match)
    {
        if (IsMatch(match, KeywordRule.PrefixMatch))
            return KeywordRule.PrefixMatch;

        if (IsMatch(match, KeywordRule.SuffixMatch))
            return KeywordRule.SuffixMatch;

        if (IsMatch(match, KeywordRule.ExactMatch))
            return KeywordRule.ExactMatch;

        return KeywordRule.ContainsMatch;
    }

    private static bool IsMode(string value, string expected)
    {
        return string.Equals(value, expected, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsMatch(string value, string expected)
    {
        return string.Equals(value, expected, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class KeywordRuleComparer : IEqualityComparer<KeywordRule>
    {
        public static KeywordRuleComparer Instance { get; } = new();

        public bool Equals(KeywordRule? x, KeywordRule? y)
        {
            if (x is null || y is null)
                return x is null && y is null;

            return string.Equals(x.Mode, y.Mode, StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.Match, y.Match, StringComparison.OrdinalIgnoreCase)
                && string.Equals(x.Term, y.Term, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(KeywordRule obj)
        {
            return HashCode.Combine(
                obj.Mode.ToUpperInvariant(),
                obj.Match.ToUpperInvariant(),
                obj.Term.ToUpperInvariant());
        }
    }
}
