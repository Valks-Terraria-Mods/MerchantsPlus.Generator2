namespace MerchantsPlus.Generator;

public sealed class MerchantSelectionSetComponent{
    public HashSet<string> BuildSelectedCategories(IEnumerable<SelectableOption> options)
    {
        return options
            .Where(option => option.IsEnabled && option.IsSelected != false)
            .Select(option => option.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public HashSet<string> BuildSelectedShops(IEnumerable<SelectableOption> options)
    {
        return options
            .Where(option => option.IsEnabled && option.IsSelected == true)
            .Select(option => option.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
