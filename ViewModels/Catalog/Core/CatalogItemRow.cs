using System.ComponentModel;
using Avalonia.Media;

namespace MerchantsPlus.Generator;

public sealed class CatalogItemRow : INotifyPropertyChanged
{
    private string _priceDisplay;
    private bool _isOverridden;
    private string _sortFlag;
    private int _conditionCount;
    private bool _isShopSlot;

    public CatalogItemRow(string name, IBrush foreground, string priceDisplay, bool isOverridden, string sortFlag, int conditionCount, bool isShopSlot = false)
    {
        Name = name;
        Foreground = foreground;
        _priceDisplay = priceDisplay;
        _isOverridden = isOverridden;
        _sortFlag = sortFlag;
        _conditionCount = Math.Max(0, conditionCount);
        _isShopSlot = isShopSlot;
    }

    public string Name { get; }
    public IBrush Foreground { get; }

    public string PriceDisplay
    {
        get => _priceDisplay;
        set
        {
            if (_priceDisplay == value)
                return;

            _priceDisplay = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PriceDisplay)));
        }
    }

    public bool IsOverridden
    {
        get => _isOverridden;
        set
        {
            if (_isOverridden == value)
                return;

            _isOverridden = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsOverridden)));
        }
    }

    public string SortFlag
    {
        get => _sortFlag;
        set
        {
            if (_sortFlag == value)
                return;

            _sortFlag = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(SortFlag)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasSortFlag)));
        }
    }

    public bool HasSortFlag => !string.IsNullOrWhiteSpace(_sortFlag);

    public int ConditionCount
    {
        get => _conditionCount;
        set
        {
            var normalized = Math.Max(0, value);
            if (_conditionCount == normalized)
                return;

            _conditionCount = normalized;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConditionCount)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ConditionFlagText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasConditions)));
        }
    }

    public string ConditionFlagText => _conditionCount > 0 ? $"Conditions ({_conditionCount})" : string.Empty;
    public bool HasConditions => _conditionCount > 0;

    public bool IsShopSlot
    {
        get => _isShopSlot;
        set
        {
            if (_isShopSlot == value)
                return;

            _isShopSlot = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsShopSlot)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
