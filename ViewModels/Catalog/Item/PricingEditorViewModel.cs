using System.Runtime.CompilerServices;

namespace MerchantsPlus.Generator;

public abstract class PricingEditorViewModel : ItemSelectionViewModel
{
    private const int MaxItemPriceCopper = 99_999_999;

    private string _pricePlatinum = "0";
    private string _priceGold = "0";
    private string _priceSilver = "0";
    private string _priceCopper = "0";
    private string _unassignedPricePlatinum = "0";
    private string _unassignedPriceGold = "0";
    private string _unassignedPriceSilver = "0";
    private string _unassignedPriceCopper = "0";
    private bool _isLoadingPriceFields;

    public bool CanResetSelectedShopPriceOverrides => !string.IsNullOrWhiteSpace(SelectedCategoryForShop) && !string.IsNullOrWhiteSpace(SelectedShopName);
    public bool CanResetSelectedCategoryPriceOverrides => !string.IsNullOrWhiteSpace(SelectedCategoryForShop);
    public string PricePlatinum { get => _pricePlatinum; set => SetPriceField(ref _pricePlatinum, value); }
    public string PriceGold { get => _priceGold; set => SetPriceField(ref _priceGold, value); }
    public string PriceSilver { get => _priceSilver; set => SetPriceField(ref _priceSilver, value); }
    public string PriceCopper { get => _priceCopper; set => SetPriceField(ref _priceCopper, value); }
    public string UnassignedPricePlatinum { get => _unassignedPricePlatinum; set => SetUnassignedPriceField(ref _unassignedPricePlatinum, value); }
    public string UnassignedPriceGold { get => _unassignedPriceGold; set => SetUnassignedPriceField(ref _unassignedPriceGold, value); }
    public string UnassignedPriceSilver { get => _unassignedPriceSilver; set => SetUnassignedPriceField(ref _unassignedPriceSilver, value); }
    public string UnassignedPriceCopper { get => _unassignedPriceCopper; set => SetUnassignedPriceField(ref _unassignedPriceCopper, value); }

    protected override void OnCatalogLoaded() { base.OnCatalogLoaded(); LoadUnassignedDefaultPrice(); }

    protected override void OnSelectedVisibleItemChanged() => LoadSelectedItemPrice();

    public void SaveSelectedItemPriceOverride()
    {
        if (SelectedVisibleItemRow is null)
            return;

        var itemName = SelectedVisibleItemRow.Name;
        var totalCopper = ApplyPriceOverrideFromFields(itemName, normalizeFields: true);
        EditorStatus = $"Saved price override for {itemName}: {ItemPrices.FormatPrice(totalCopper)}";
    }

    public void ResetSelectedItemPriceOverride()
    {
        if (SelectedVisibleItemRow is null)
            return;

        var itemName = SelectedVisibleItemRow.Name;
        ItemPrices.ClearOverride(itemName);
        UpdateVisibleItemRow(itemName);
        LoadSelectedItemPrice();
        EditorStatus = $"Removed custom price override for {itemName}.";
    }

    public void ResetSelectedShopPriceOverrides()
    {
        var category = ItemCatalog.Categories.FirstOrDefault(c => string.Equals(c.Name, SelectedCategoryForShop, StringComparison.OrdinalIgnoreCase));
        var shop = category?.Shops.FirstOrDefault(s => string.Equals(s.Name, SelectedShopName, StringComparison.OrdinalIgnoreCase));
        if (shop is null)
            return;

        var removed = ResetOverridesForItems(shop.Items);
        EditorStatus = removed > 0 ? $"Reset {removed} overridden item prices for shop {shop.Name}." : $"No custom price overrides found for shop {shop.Name}.";
    }

    public void ResetSelectedCategoryPriceOverrides()
    {
        var category = ItemCatalog.Categories.FirstOrDefault(c => string.Equals(c.Name, SelectedCategoryForShop, StringComparison.OrdinalIgnoreCase));
        if (category is null)
            return;

        var removed = ResetOverridesForItems(category.Shops.SelectMany(s => s.Items));
        EditorStatus = removed > 0 ? $"Reset {removed} overridden item prices for category {category.Name}." : $"No custom price overrides found for category {category.Name}.";
    }

    public void SaveUnassignedDefaultPrice()
    {
        var totalCopper = ApplyUnassignedDefaultPriceFromFields(normalizeFields: true);
        SaveMerchantAssignmentsAndOutput();
        RefreshVisibleItemPrices();
        LoadSelectedItemPrice();
        EditorStatus = totalCopper > 0
            ? $"Saved default price for unassigned items: {ItemPrices.FormatPrice(totalCopper)}"
            : "Cleared default price for unassigned items.";
    }

    public void ResetUnassignedDefaultPrice()
    {
        SetUnassignedPriceFieldsFromCopper(0);
        ItemPrices.SetUnassignedDefaultPriceCopper(0);
        SaveMerchantAssignmentsAndOutput();
        RefreshVisibleItemPrices();
        LoadSelectedItemPrice();
        EditorStatus = "Cleared default price for unassigned items.";
    }

    protected void LoadSelectedItemPrice() => SetPriceFieldsFromCopper(SelectedVisibleItemRow is null ? 0 : ItemPrices.GetPriceCopper(SelectedVisibleItemRow.Name));

    private void SetPriceField(ref string backingField, string value, [CallerMemberName] string propertyName = "")
    {
        var normalized = PriceEditorCurrency.NormalizeField(value);
        if (backingField == normalized)
            return;

        backingField = normalized;
        OnPropertyChanged(propertyName);

        if (!_isLoadingPriceFields && SelectedVisibleItemRow is not null)
            ApplyPriceOverrideFromFields(SelectedVisibleItemRow.Name, normalizeFields: true);
    }

    private void SetUnassignedPriceField(ref string backingField, string value, [CallerMemberName] string propertyName = "")
    {
        var normalized = PriceEditorCurrency.NormalizeField(value);
        if (backingField == normalized)
            return;

        backingField = normalized;
        OnPropertyChanged(propertyName);
    }

    private int ApplyPriceOverrideFromFields(string itemName, bool normalizeFields)
    {
        var totalCopper = PriceEditorCurrency.ParseNormalizedCopper(_pricePlatinum, _priceGold, _priceSilver, _priceCopper, MaxItemPriceCopper);
        if (normalizeFields)
            SetPriceFieldsFromCopper(totalCopper);

        ItemPrices.SetOverride(itemName, totalCopper);
        UpdateVisibleItemRow(itemName);
        return totalCopper;
    }

    private int ApplyUnassignedDefaultPriceFromFields(bool normalizeFields)
    {
        var totalCopper = PriceEditorCurrency.ParseNormalizedCopper(_unassignedPricePlatinum, _unassignedPriceGold, _unassignedPriceSilver, _unassignedPriceCopper, MaxItemPriceCopper);
        if (normalizeFields)
            SetUnassignedPriceFieldsFromCopper(totalCopper);

        ItemPrices.SetUnassignedDefaultPriceCopper(totalCopper);
        return totalCopper;
    }

    private int ResetOverridesForItems(IEnumerable<string> itemNames)
    {
        var names = itemNames.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var removed = ItemPrices.ClearOverrides(names);
        if (removed == 0)
            return 0;

        foreach (var name in names)
            UpdateVisibleItemRow(name);

        if (SelectedVisibleItemRow is not null && names.Contains(SelectedVisibleItemRow.Name, StringComparer.OrdinalIgnoreCase))
            LoadSelectedItemPrice();

        return removed;
    }

    private void SetPriceFieldsFromCopper(int totalCopper)
    {
        var (platinum, gold, silver, copper) = ItemPrices.ToCurrency(totalCopper);
        _isLoadingPriceFields = true;
        _pricePlatinum = platinum.ToString();
        _priceGold = gold.ToString();
        _priceSilver = silver.ToString();
        _priceCopper = copper.ToString();
        _isLoadingPriceFields = false;

        OnPropertyChanged(nameof(PricePlatinum));
        OnPropertyChanged(nameof(PriceGold));
        OnPropertyChanged(nameof(PriceSilver));
        OnPropertyChanged(nameof(PriceCopper));
    }

    private void SetUnassignedPriceFieldsFromCopper(int totalCopper)
    {
        var (platinum, gold, silver, copper) = ItemPrices.ToCurrency(totalCopper);
        _unassignedPricePlatinum = platinum.ToString();
        _unassignedPriceGold = gold.ToString();
        _unassignedPriceSilver = silver.ToString();
        _unassignedPriceCopper = copper.ToString();

        OnPropertyChanged(nameof(UnassignedPricePlatinum));
        OnPropertyChanged(nameof(UnassignedPriceGold));
        OnPropertyChanged(nameof(UnassignedPriceSilver));
        OnPropertyChanged(nameof(UnassignedPriceCopper));
    }

    protected void LoadUnassignedDefaultPrice() => SetUnassignedPriceFieldsFromCopper(ItemPrices.GetUnassignedDefaultPriceCopper());

    protected void RefreshVisibleItemPrices()
    {
        foreach (var row in VisibleItems) UpdateVisibleItemRow(row.Name);
    }
}
