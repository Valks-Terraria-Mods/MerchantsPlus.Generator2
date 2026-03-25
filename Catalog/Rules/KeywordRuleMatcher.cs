namespace MerchantsPlus.Generator;

public static class KeywordRuleMatcher
{
    public static bool MatchesAny(string item, IEnumerable<KeywordRule> rules, string mode)
    {
        foreach (var rule in rules)
        {
            if (!string.Equals(rule.Mode, mode, StringComparison.OrdinalIgnoreCase))
                continue;

            if (IsMatch(item, rule))
                return true;
        }

        return false;
    }

    public static bool IsMatch(string item, KeywordRule rule)
    {
        if (string.IsNullOrWhiteSpace(rule.Term))
            return false;

        var term = rule.Term.Trim();

        if (string.Equals(rule.Match, KeywordRule.PrefixMatch, StringComparison.OrdinalIgnoreCase))
            return item.StartsWith(term, StringComparison.OrdinalIgnoreCase);

        if (string.Equals(rule.Match, KeywordRule.SuffixMatch, StringComparison.OrdinalIgnoreCase))
            return item.EndsWith(term, StringComparison.OrdinalIgnoreCase);

        if (string.Equals(rule.Match, KeywordRule.ExactMatch, StringComparison.OrdinalIgnoreCase))
            return string.Equals(item, term, StringComparison.OrdinalIgnoreCase);

        return item.Contains(term, StringComparison.OrdinalIgnoreCase);
    }
}
