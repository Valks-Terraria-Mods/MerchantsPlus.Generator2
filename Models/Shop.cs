using System.Text;

namespace MerchantsPlus.Generator;

public class Shop{
    public required string Name { get; set; }
    public int Priority { get; set; }
    public required string[] Keywords { get; set; }
    public KeywordRule[] KeywordRules { get; set; } = [];
    public string[] FirstOrder { get; set; } = [];
    public string[] LastOrder { get; set; } = [];
    public string[] ExcludedItems { get; set; } = [];
    public string SharedOrderSourceShopName { get; set; } = string.Empty;
    public string ParentShopName { get; set; } = string.Empty;
    public string[] CategoryKeywords { get; set; } = [];
    public string[] CategoryBlacklistedKeywords { get; set; } = [];
    public KeywordRule[] CategoryKeywordRules { get; set; } = [];
    public string[] CategoryFirstOrder { get; set; } = [];
    public string[] CategoryLastOrder { get; set; } = [];
    public string[] GlobalFirstOrder { get; set; } = [];
    public string[] GlobalLastOrder { get; set; } = [];
    public IReadOnlyList<string> Items => _items;

    private readonly List<string> _items = [];
    private readonly List<(int Bucket, int Index)> _itemSortKeys = [];

    public void AddItem(string item)
    {
        var sortKey = GetSortKey(item);
        var insertIndex = 0;

        while (insertIndex < _itemSortKeys.Count && CompareSortKeys(_itemSortKeys[insertIndex], sortKey) <= 0)
        {
            insertIndex++;
        }

        _items.Insert(insertIndex, item);
        _itemSortKeys.Insert(insertIndex, sortKey);
    }

    public void Clear()
    {
        _items.Clear();
        _itemSortKeys.Clear();
    }

    private (int Bucket, int Index) GetSortKey(string item)
    {
        var effectiveFirstOrder = FirstOrder.Length > 0 ? FirstOrder : CategoryFirstOrder;
        var effectiveLastOrder = LastOrder.Length > 0 ? LastOrder : CategoryLastOrder;

        var globalFirstIndex = FindOrderIndex(item, GlobalFirstOrder, startsWithFirst: true);
        if (globalFirstIndex >= 0)
            return (0, globalFirstIndex);

        var globalLastIndex = FindOrderIndex(item, GlobalLastOrder, startsWithFirst: false);
        if (globalLastIndex >= 0)
            return (2, globalLastIndex);

        var firstIndex = FindOrderIndex(item, effectiveFirstOrder, startsWithFirst: true);
        if (firstIndex >= 0)
            return (0, GlobalFirstOrder.Length + firstIndex);

        var lastIndex = FindOrderIndex(item, effectiveLastOrder, startsWithFirst: false);
        if (lastIndex >= 0)
            return (2, GlobalLastOrder.Length + lastIndex);

        // Items that do not match a first/last term stay in the middle bucket.
        return (1, 0);
    }

    private static int CompareSortKeys((int Bucket, int Index) left, (int Bucket, int Index) right)
    {
        var bucketComparison = left.Bucket.CompareTo(right.Bucket);
        if (bucketComparison != 0)
            return bucketComparison;

        return left.Index.CompareTo(right.Index);
    }

    private static int FindOrderIndex(string item, string[] orderedTerms, bool startsWithFirst)
    {
        for (var i = 0; i < orderedTerms.Length; i++)
        {
            var isMatch = startsWithFirst && i == 0
                ? item.StartsWith(orderedTerms[i], StringComparison.OrdinalIgnoreCase)
                : item.Contains(orderedTerms[i], StringComparison.OrdinalIgnoreCase);

            if (isMatch)
                return i;
        }

        return -1;
    }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine($"  {Name}");
        foreach (var item in _items)
        {
            sb.AppendLine($"    {item}");
        }

        return sb.ToString();
    }
}
