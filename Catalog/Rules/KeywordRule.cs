namespace MerchantsPlus.Generator;

public sealed class KeywordRule{
    public const string WhitelistMode = "Whitelist";
    public const string BlacklistMode = "Blacklist";
    public const string ContainsMatch = "Contains";
    public const string PrefixMatch = "Prefix";
    public const string SuffixMatch = "Suffix";
    public const string ExactMatch = "Exact";

    public string Mode { get; set; } = WhitelistMode;
    public string Match { get; set; } = ContainsMatch;
    public string Term { get; set; } = string.Empty;
}
