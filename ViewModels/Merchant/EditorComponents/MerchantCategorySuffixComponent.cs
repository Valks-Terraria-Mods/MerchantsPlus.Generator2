namespace MerchantsPlus.Generator;

public sealed class MerchantCategorySuffixComponent{
    public string BuildCategoryMerchantSuffix(
        string categoryName,
        IReadOnlyDictionary<string, MerchantAssignment> merchantAssignments,
        Func<MerchantAssignment, MerchantAssignment> buildEffectiveAssignment)
    {
        var merchants = merchantAssignments
            .Where(entry =>
            {
                var assignment = buildEffectiveAssignment(entry.Value);
                return assignment.Categories.Contains(categoryName)
                    || assignment.Shops.Any(shop => shop.StartsWith($"{categoryName} / ", StringComparison.OrdinalIgnoreCase));
            })
            .Select(entry => entry.Key)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (merchants.Length == 0)
            return string.Empty;

        var shown = merchants.Take(4).ToArray();
        return merchants.Length > 4 ? $"({string.Join(", ", shown)}, ...)" : $"({string.Join(", ", shown)})";
    }
}
