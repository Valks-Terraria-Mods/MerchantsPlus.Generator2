namespace MerchantsPlus.Generator;

public static class ItemPrices
{
    private static readonly HashSet<int> _knownSetDefaultsNullRefIds =
    [
        269, 270, 271,
        5104, 5105, 5106,
        5136, 5305
    ];

    private static readonly Dictionary<string, int> _defaultPrices = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, int> _overridePrices = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, int> _itemIdByName = new(StringComparer.OrdinalIgnoreCase);
    private static readonly HashSet<int> _failedDefaultPriceIds = [];
    private static int _unassignedDefaultPriceCopper;
    private static bool _loaded;
    private static Type? _itemType;
    private static Func<object>? _itemFactory;
    private static Action<object, int, bool>? _setDefaultsSafeAction;
    private static Action<object, int>? _setDefaultsAction;
    private static Func<object, int>? _valueGetter;

    public static int GetPriceCopper(string itemName)
    {
        EnsureLoaded();

        if (_overridePrices.TryGetValue(itemName, out var overrideValue))
            return overrideValue;

        if (_defaultPrices.TryGetValue(itemName, out var value))
            return ApplyUnassignedDefaultPrice(value);

        if (!_itemIdByName.TryGetValue(itemName, out var itemId))
            return ApplyUnassignedDefaultPrice(0);

        var resolved = ResolveDefaultPrice(itemId);
        _defaultPrices[itemName] = resolved;
        return ApplyUnassignedDefaultPrice(resolved);
    }

    public static int GetCachedPriceCopper(string itemName)
    {
        EnsureLoaded();

        if (_overridePrices.TryGetValue(itemName, out var overrideValue))
            return overrideValue;

        return _defaultPrices.TryGetValue(itemName, out var value) ? ApplyUnassignedDefaultPrice(value) : ApplyUnassignedDefaultPrice(0);
    }

    public static int GetUnassignedDefaultPriceCopper()
    {
        EnsureLoaded();
        return _unassignedDefaultPriceCopper;
    }

    public static void SetUnassignedDefaultPriceCopper(int copper)
    {
        EnsureLoaded();
        _unassignedDefaultPriceCopper = Math.Max(0, copper);
        CatalogStorage.SaveUnassignedDefaultPrice(_unassignedDefaultPriceCopper);
    }

    public static bool IsOverridden(string itemName)
    {
        EnsureLoaded();
        return _overridePrices.ContainsKey(itemName);
    }

    public static void SetOverride(string itemName, int copper)
    {
        EnsureLoaded();
        _overridePrices[itemName] = Math.Max(0, copper);
        CatalogStorage.SaveItemPriceOverrides(_overridePrices);
    }

    public static void ClearOverride(string itemName)
    {
        EnsureLoaded();
        _overridePrices.Remove(itemName);
        CatalogStorage.SaveItemPriceOverrides(_overridePrices);
    }

    public static int ClearOverrides(IEnumerable<string> itemNames)
    {
        EnsureLoaded();

        var removed = 0;
        foreach (var itemName in itemNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (_overridePrices.Remove(itemName))
                removed++;
        }

        if (removed > 0)
            CatalogStorage.SaveItemPriceOverrides(_overridePrices);

        return removed;
    }

    public static string FormatPrice(int copper)
    {
        if (copper <= 0)
            return string.Empty;

        var (plat, gold, silver, cop) = ToCurrency(copper);
        var parts = new List<string>(4);
        if (plat > 0) parts.Add($"{plat}p");
        if (gold > 0) parts.Add($"{gold}g");
        if (silver > 0) parts.Add($"{silver}s");
        if (cop > 0 || parts.Count == 0) parts.Add($"{cop}c");
        return string.Join(" ", parts);
    }

    public static (int platinum, int gold, int silver, int copper) ToCurrency(int totalCopper)
    {
        totalCopper = Math.Max(0, totalCopper);
        var platinum = totalCopper / 1_000_000;
        totalCopper %= 1_000_000;
        var gold = totalCopper / 10_000;
        totalCopper %= 10_000;
        var silver = totalCopper / 100;
        var copper = totalCopper % 100;
        return (platinum, gold, silver, copper);
    }

    public static int ToCopper(int platinum, int gold, int silver, int copper)
    {
        return Math.Max(0, platinum) * 1_000_000
            + Math.Max(0, gold) * 10_000
            + Math.Max(0, silver) * 100
            + Math.Max(0, copper);
    }

    private static void EnsureLoaded()
    {
        if (_loaded)
            return;

        ItemPriceMetadataLoader.LoadTypeAndIdMetadata(
            _itemIdByName,
            out _itemType,
            out _itemFactory,
            out _setDefaultsSafeAction,
            out _setDefaultsAction,
            out _valueGetter);

        foreach (var entry in CatalogStorage.LoadItemPriceOverrides())
            _overridePrices[entry.Key] = Math.Max(0, entry.Value);

        _unassignedDefaultPriceCopper = Math.Max(0, CatalogStorage.LoadUnassignedDefaultPrice());

        _loaded = true;
    }

    private static int ApplyUnassignedDefaultPrice(int value)
    {
        return value > 0 ? value : _unassignedDefaultPriceCopper;
    }

    private static int ResolveDefaultPrice(int itemId)
    {
        return ItemPriceMetadataLoader.ResolveDefaultPrice(
            itemId,
            _knownSetDefaultsNullRefIds,
            _failedDefaultPriceIds,
            _itemType,
            _itemFactory,
            _setDefaultsSafeAction,
            _setDefaultsAction,
            _valueGetter);
    }

}
