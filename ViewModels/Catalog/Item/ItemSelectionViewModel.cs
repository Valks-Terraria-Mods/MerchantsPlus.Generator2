namespace MerchantsPlus.Generator;

public abstract class ItemSelectionViewModel : OverviewViewModel
{
    private const string SingleViewMode = "Single View";
    private CatalogItemRow? _selectedVisibleItem;

    public CatalogItemRow? SelectedVisibleItem
    {
        get => _selectedVisibleItem;
        set
        {
            if (_selectedVisibleItem == value)
                return;

            _selectedVisibleItem = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedItemName));
            OnPropertyChanged(nameof(IsPriceEditorEnabled));
            OnPropertyChanged(nameof(IsPriceEditorReadOnly));
            OnPropertyChanged(nameof(IsConditionEditorEnabled));
            OnPropertyChanged(nameof(IsConditionEditorReadOnly));
            OnSelectedVisibleItemChanged();
        }
    }

    public string SelectedItemName => _selectedVisibleItem?.Name ?? "No item selected";
    public bool IsPriceEditorEnabled => _selectedVisibleItem is not null;
    public bool IsPriceEditorReadOnly => _selectedVisibleItem is null;
    public bool IsConditionEditorEnabled => _selectedVisibleItem is not null;
    public bool IsConditionEditorReadOnly => _selectedVisibleItem is null;

    protected CatalogItemRow? SelectedVisibleItemRow => _selectedVisibleItem;

    protected override void OnMiddlePanelSplitModeChanged(string previousMode, string nextMode)
    {
        var wasSplit = IsSplitMode(previousMode);
        var isSplit = IsSplitMode(nextMode);

        if (!wasSplit && isSplit)
        {
            CaptureSecondaryPanelSnapshot();
            return;
        }

        if (wasSplit && !isSplit)
            ClearSecondaryPanelSnapshot();
    }

    protected virtual void OnSelectedVisibleItemChanged()
    {
    }

    private void CaptureSecondaryPanelSnapshot()
    {
        SecondaryVisibleItems.Clear();
        foreach (var row in VisibleItems)
            SecondaryVisibleItems.Add(CloneItemRow(row));

        SetSecondaryPanelTitle(GetCurrentShopViewTitle());
    }

    private void ClearSecondaryPanelSnapshot()
    {
        SecondaryVisibleItems.Clear();
        SetSecondaryPanelTitle(string.Empty);
    }

    private static bool IsSplitMode(string mode)
    {
        return !string.Equals(mode, SingleViewMode, StringComparison.OrdinalIgnoreCase);
    }

    private static CatalogItemRow CloneItemRow(CatalogItemRow source)
    {
        return new CatalogItemRow(
            source.Name,
            source.Foreground,
            source.PriceDisplay,
            source.IsOverridden,
            source.SortFlag,
            source.ConditionCount,
            source.IsShopSlot);
    }

    protected void UpdateVisibleItemRow(string itemName)
    {
        var row = VisibleItems.FirstOrDefault(v => string.Equals(v.Name, itemName, StringComparison.OrdinalIgnoreCase));
        if (row is null)
            return;

        row.PriceDisplay = ItemPrices.FormatPrice(ItemPrices.GetPriceCopper(itemName));
        row.IsOverridden = ItemPrices.IsOverridden(itemName);
        row.ConditionCount = ItemConditions.GetConditions(itemName).Length;
    }
}
