namespace MerchantsPlus.Generator;

public static class ItemConditions
{
    private static readonly Dictionary<string, HashSet<string>> _conditionsByItem = new(StringComparer.OrdinalIgnoreCase);
    private static bool _loaded;

    public static string[] GetConditions(string itemName)
    {
        EnsureLoaded();

        if (_conditionsByItem.TryGetValue(itemName, out var conditions))
        {
            return
            [
                .. conditions.OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
            ];
        }

        return [];
    }

    public static void SetConditions(string itemName, IEnumerable<string> conditions)
    {
        EnsureLoaded();

        var normalized = conditions
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(c => c, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalized.Length == 0)
        {
            _conditionsByItem.Remove(itemName);
        }
        else
        {
            _conditionsByItem[itemName] = new HashSet<string>(normalized, StringComparer.OrdinalIgnoreCase);
        }

        Save();
    }

    public static void ClearConditions(string itemName)
    {
        EnsureLoaded();

        if (!_conditionsByItem.Remove(itemName))
            return;

        Save();
    }

    public static int ClearConditions(IEnumerable<string> itemNames)
    {
        EnsureLoaded();

        var removed = 0;
        foreach (var itemName in itemNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (_conditionsByItem.Remove(itemName))
                removed++;
        }

        if (removed > 0)
            Save();

        return removed;
    }

    private static void EnsureLoaded()
    {
        if (_loaded)
            return;

        _conditionsByItem.Clear();
        foreach (var entry in CatalogStorage.LoadItemConditions())
        {
            var values = entry.Value
                .Where(v => !string.IsNullOrWhiteSpace(v))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (values.Length == 0)
                continue;

            _conditionsByItem[entry.Key] = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
        }

        _loaded = true;
    }

    private static void Save()
    {
        var data = _conditionsByItem.ToDictionary(
            entry => entry.Key,
            entry => entry.Value.OrderBy(v => v, StringComparer.OrdinalIgnoreCase).ToArray(),
            StringComparer.OrdinalIgnoreCase);

        CatalogStorage.SaveItemConditions(data);
    }
}
