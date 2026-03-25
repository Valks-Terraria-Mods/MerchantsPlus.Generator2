using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MerchantsPlus.Generator;

public abstract class ConditionEditorViewModel : GlobalFlagsViewModel
{
    private bool _isLoadingConditionFields;
    private string _conditionFilterText = string.Empty;

    public bool CanResetSelectedShopConditionOverrides => !string.IsNullOrWhiteSpace(SelectedCategoryForShop) && !string.IsNullOrWhiteSpace(SelectedShopName);
    public bool CanResetSelectedCategoryConditionOverrides => !string.IsNullOrWhiteSpace(SelectedCategoryForShop);
    public ObservableCollection<SelectableOption> ItemConditionOptions { get; } = [];
    public ObservableCollection<SelectableOption> FilteredItemConditionOptions { get; } = [];

    public string ConditionFilterText
    {
        get => _conditionFilterText;
        set
        {
            if (_conditionFilterText == value)
                return;

            _conditionFilterText = value;
            OnPropertyChanged();
            RefreshFilteredConditionOptions();
        }
    }

    protected override void OnSelectedVisibleItemChanged()
    {
        base.OnSelectedVisibleItemChanged();
        LoadSelectedItemConditions();
    }

    protected void InitializeConditionOptions()
    {
        ItemConditionOptions.Clear();

        try
        {
            foreach (var flag in ProgressionFlags.GetWorldProgressionFlags())
            {
                var option = new SelectableOption(flag, flag, isSelected: false, isEnabled: true);
                option.PropertyChanged += OnConditionOptionPropertyChanged;
                ItemConditionOptions.Add(option);
            }
        }
        catch (Exception ex)
        {
            EditorStatus = $"Failed to load progression conditions: {ex.Message}";
        }

        RefreshFilteredConditionOptions();
        LoadSelectedItemConditions();
    }

    private void OnConditionOptionPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SelectableOption.IsSelected) || _isLoadingConditionFields || SelectedVisibleItemRow is null)
            return;

        var selectedFlags = ItemConditionOptions
            .Where(o => o.IsSelected == true)
            .Select(o => o.Key)
            .ToArray();

        ItemConditions.SetConditions(SelectedVisibleItemRow.Name, selectedFlags);
        UpdateVisibleItemRow(SelectedVisibleItemRow.Name);
    }

    public void ResetSelectedItemConditionOverride()
    {
        if (SelectedVisibleItemRow is null)
            return;

        var itemName = SelectedVisibleItemRow.Name;
        ItemConditions.ClearConditions(itemName);
        UpdateVisibleItemRow(itemName);
        LoadSelectedItemConditions();
        EditorStatus = $"Removed custom conditions for {itemName}.";
    }

    public void ResetSelectedShopConditionOverrides()
    {
        var category = ItemCatalog.Categories.FirstOrDefault(c => string.Equals(c.Name, SelectedCategoryForShop, StringComparison.OrdinalIgnoreCase));
        var shop = category?.Shops.FirstOrDefault(s => string.Equals(s.Name, SelectedShopName, StringComparison.OrdinalIgnoreCase));
        if (shop is null)
            return;

        var removed = ResetConditionsForItems(shop.Items);
        EditorStatus = removed > 0 ? $"Reset {removed} overridden item conditions for shop {shop.Name}." : $"No custom condition overrides found for shop {shop.Name}.";
    }

    public void ResetSelectedCategoryConditionOverrides()
    {
        var category = ItemCatalog.Categories.FirstOrDefault(c => string.Equals(c.Name, SelectedCategoryForShop, StringComparison.OrdinalIgnoreCase));
        if (category is null)
            return;

        var removed = ResetConditionsForItems(category.Shops.SelectMany(s => s.Items));
        EditorStatus = removed > 0 ? $"Reset {removed} overridden item conditions for category {category.Name}." : $"No custom condition overrides found for category {category.Name}.";
    }

    public void ResetAllConditionOverrides()
    {
        var allShopItems = ItemCatalog.Categories
            .SelectMany(category => category.Shops)
            .Where(shop => !string.Equals(shop.Name, "Unorganized", StringComparison.OrdinalIgnoreCase))
            .SelectMany(shop => shop.Items)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var removed = ItemConditions.ClearConditions(allShopItems);
        foreach (var row in VisibleItems)
            UpdateVisibleItemRow(row.Name);

        EditorStatus = $"Reset all condition overrides across shops ({removed} items).";
    }

    private int ResetConditionsForItems(IEnumerable<string> itemNames)
    {
        var names = itemNames.Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var removed = ItemConditions.ClearConditions(names);
        if (removed == 0)
            return 0;

        foreach (var name in names)
            UpdateVisibleItemRow(name);

        if (SelectedVisibleItemRow is not null && names.Contains(SelectedVisibleItemRow.Name, StringComparer.OrdinalIgnoreCase))
            LoadSelectedItemConditions();

        return removed;
    }

    private void RefreshFilteredConditionOptions()
    {
        FilteredItemConditionOptions.Clear();

        foreach (var option in ItemConditionOptions)
        {
            if (string.IsNullOrWhiteSpace(_conditionFilterText) || option.Label.Contains(_conditionFilterText, StringComparison.OrdinalIgnoreCase))
                FilteredItemConditionOptions.Add(option);
        }
    }

    private void LoadSelectedItemConditions()
    {
        _isLoadingConditionFields = true;
        try
        {
            var selected = SelectedVisibleItemRow is null ? [] : ItemConditions.GetConditions(SelectedVisibleItemRow.Name);
            var selectedSet = new HashSet<string>(selected, StringComparer.OrdinalIgnoreCase);
            foreach (var option in ItemConditionOptions)
                option.IsSelected = selectedSet.Contains(option.Key);
        }
        finally
        {
            _isLoadingConditionFields = false;
        }
    }
}
