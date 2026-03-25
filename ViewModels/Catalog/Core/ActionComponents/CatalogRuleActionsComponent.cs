using System.Collections.ObjectModel;

namespace MerchantsPlus.Generator;

public sealed class CatalogRuleActionsComponent{
    public void AddRule(
        ObservableCollection<KeywordRuleEditorRow> target,
        string mode,
        string match,
        string term,
        Action<string> clearTerm)
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
