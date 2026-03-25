namespace MerchantsPlus.Generator;

public abstract class MerchantRulesViewModel : MerchantEditorViewModel
{
    protected override List<string> FindConflictsWithOtherMerchants(string merchantName, HashSet<string> categories, HashSet<string> shops)
    {
        var conflicts = new List<string>();

        foreach (var entry in MerchantAssignments)
        {
            if (string.Equals(entry.Key, merchantName, StringComparison.OrdinalIgnoreCase))
                continue;

            var other = BuildEffectiveAssignment(entry.Value);
            var categoryOverlap = categories.Intersect(other.Categories, StringComparer.OrdinalIgnoreCase).Take(2).ToArray();
            var shopOverlap = shops.Intersect(other.Shops, StringComparer.OrdinalIgnoreCase).Take(2).ToArray();

            if (categoryOverlap.Length > 0)
                conflicts.Add($"categories {string.Join(", ", categoryOverlap)} already assigned to {entry.Key}");

            if (shopOverlap.Length > 0)
                conflicts.Add($"shops {string.Join(", ", shopOverlap)} already assigned to {entry.Key}");
        }

        return conflicts;
    }

    protected override Dictionary<string, List<string>> GetCategoryOwners(string selectedMerchant)
    {
        var owners = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in MerchantAssignments)
        {
            if (string.Equals(entry.Key, selectedMerchant, StringComparison.OrdinalIgnoreCase))
                continue;

            foreach (var category in BuildEffectiveAssignment(entry.Value).Categories)
            {
                if (!owners.TryGetValue(category, out var list))
                {
                    list = [];
                    owners[category] = list;
                }

                list.Add(entry.Key);
            }
        }

        return owners;
    }

    protected override Dictionary<string, List<string>> GetShopOwners(string selectedMerchant)
    {
        var owners = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase);

        foreach (var entry in MerchantAssignments)
        {
            if (string.Equals(entry.Key, selectedMerchant, StringComparison.OrdinalIgnoreCase))
                continue;

            foreach (var shop in BuildEffectiveAssignment(entry.Value).Shops)
            {
                if (!owners.TryGetValue(shop, out var list))
                {
                    list = [];
                    owners[shop] = list;
                }

                list.Add(entry.Key);
            }
        }

        return owners;
    }

    protected override void UpdateCategoryStateFromChildren(SelectableOption category, IEnumerable<SelectableOption> children)
    {
        var enabledChildren = children.Where(v => v.IsEnabled).ToArray();
        if (enabledChildren.Length == 0)
            return;

        var selectedCount = enabledChildren.Count(v => v.IsSelected == true);
        category.IsSelected = selectedCount == 0
            ? false
            : selectedCount == enabledChildren.Length;

        if (selectedCount > 0 && selectedCount < enabledChildren.Length)
            category.IsSelected = null;
    }

    protected static string FormatOwners(IEnumerable<string> owners)
    {
        return string.Join(", ", owners.Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(v => v, StringComparer.OrdinalIgnoreCase));
    }
}
