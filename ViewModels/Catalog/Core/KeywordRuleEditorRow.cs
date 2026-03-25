using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MerchantsPlus.Generator;

public sealed class KeywordRuleEditorRow : INotifyPropertyChanged
{
    private string _mode = KeywordRule.WhitelistMode;
    private string _match = KeywordRule.ContainsMatch;
    private string _term = string.Empty;

    public string Mode
    {
        get => _mode;
        set => SetField(ref _mode, value);
    }

    public string Match
    {
        get => _match;
        set => SetField(ref _match, value);
    }

    public string Term
    {
        get => _term;
        set => SetField(ref _term, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public KeywordRule ToRule()
    {
        return new KeywordRule
        {
            Mode = Mode,
            Match = Match,
            Term = Term
        };
    }

    public static KeywordRuleEditorRow FromRule(KeywordRule rule)
    {
        return new KeywordRuleEditorRow
        {
            Mode = rule.Mode,
            Match = rule.Match,
            Term = rule.Term
        };
    }

    private void SetField(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
            return;

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
