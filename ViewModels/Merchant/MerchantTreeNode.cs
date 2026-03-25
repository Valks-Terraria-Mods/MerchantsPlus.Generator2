using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MerchantsPlus.Generator;

public sealed class MerchantTreeNode : INotifyPropertyChanged
{
    private bool _isExpanded;
    private string _itemCountBadgeText = string.Empty;
    private string _shopCountBadgeText = string.Empty;

    public MerchantTreeNode(string title, string? categoryName = null, string? shopName = null)
    {
        Title = title;
        CategoryName = categoryName;
        ShopName = shopName;
    }

    public string Title { get; }
    public string? CategoryName { get; }
    public string? ShopName { get; }
    public ObservableCollection<MerchantTreeNode> Children { get; } = [];
    public ObservableCollection<MerchantShopBadgeRow> ShopBadges { get; } = [];
    public bool HasItemCountBadge => !string.IsNullOrWhiteSpace(_itemCountBadgeText);
    public bool HasShopCountBadge => !string.IsNullOrWhiteSpace(_shopCountBadgeText);

    public string ItemCountBadgeText
    {
        get => _itemCountBadgeText;
        set
        {
            if (_itemCountBadgeText == value)
                return;

            _itemCountBadgeText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ItemCountBadgeText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasItemCountBadge)));
        }
    }

    public string ShopCountBadgeText
    {
        get => _shopCountBadgeText;
        set
        {
            if (_shopCountBadgeText == value)
                return;

            _shopCountBadgeText = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShopCountBadgeText)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasShopCountBadge)));
        }
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set
        {
            if (_isExpanded == value)
                return;

            _isExpanded = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsExpanded)));
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}
