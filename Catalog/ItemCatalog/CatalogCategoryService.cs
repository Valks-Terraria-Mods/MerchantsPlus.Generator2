namespace MerchantsPlus.Generator;

public class CatalogCategoryService{
    private readonly ItemCatalogState _state;
    private readonly CatalogConfigurationSupport _configurationSupport;

    public CatalogCategoryService(ItemCatalogState state, CatalogConfigurationSupport configurationSupport)
    {
        _state = state;
        _configurationSupport = configurationSupport;
    }

    public bool AddCategory(string categoryName, IEnumerable<KeywordRule>? rules = null, string[]? firstOrder = null, string[]? lastOrder = null)
    {
        if (string.IsNullOrWhiteSpace(categoryName))
            return false;

        if (_state.Categories.Any(category => string.Equals(category.Name, categoryName, StringComparison.OrdinalIgnoreCase)))
            return false;

        var normalizedRules = KeywordRuleSet.Normalize(rules ?? []);

        _state.MutableCategories.Add(new Category
        {
            Name = categoryName.Trim(),
            KeywordRules = normalizedRules,
            Keywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.WhitelistMode),
            BlacklistedKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.BlacklistMode),
            FirstOrder = firstOrder ?? [],
            LastOrder = lastOrder ?? [],
            Shops = []
        });

        _configurationSupport.SaveConfiguration();
        return true;
    }

    public bool UpdateCategory(string existingName, string newName, IEnumerable<KeywordRule>? rules = null, string[]? firstOrder = null, string[]? lastOrder = null)
    {
        var category = _state.Categories.FirstOrDefault(candidate => string.Equals(candidate.Name, existingName, StringComparison.OrdinalIgnoreCase));
        if (category is null || string.IsNullOrWhiteSpace(newName))
            return false;

        if (_state.Categories.Any(candidate => !ReferenceEquals(candidate, category)
            && string.Equals(candidate.Name, newName, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var normalizedRules = KeywordRuleSet.Normalize(rules ?? []);

        category.Name = newName.Trim();
        category.KeywordRules = normalizedRules;
        category.Keywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.WhitelistMode);
        category.BlacklistedKeywords = KeywordRuleSet.ToLegacyContains(normalizedRules, KeywordRule.BlacklistMode);
        category.FirstOrder = firstOrder ?? [];
        category.LastOrder = lastOrder ?? [];

        _configurationSupport.ApplyCategoryAndSharedOrderLinks(category);
        _configurationSupport.SaveConfiguration();
        return true;
    }

    public bool RemoveCategory(string categoryName)
    {
        var category = _state.Categories.FirstOrDefault(candidate => string.Equals(candidate.Name, categoryName, StringComparison.OrdinalIgnoreCase));
        if (category is null)
            return false;

        _state.MutableCategories.Remove(category);
        _configurationSupport.SaveConfiguration();
        return true;
    }
}