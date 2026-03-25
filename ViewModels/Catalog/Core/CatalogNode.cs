using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MerchantsPlus.Generator;

public class CatalogNode : INotifyPropertyChanged
{
    private bool _isExpanded;
    private string _itemCountBadgeText = string.Empty;
    private string _shopCountBadgeText = string.Empty;

    public CatalogNode(string title, CatalogNodeType type, IEnumerable<string>? items = null, string editorShopName = "", int? slotCount = null)
    {
        Title = title;
        Type = type;
        EditorShopName = editorShopName;

        if (items is not null)
            Items = [.. items];

        SlotCount = slotCount ?? Items.Count;
    }

    public string Title { get; }
    public CatalogNodeType Type { get; }
    public string EditorShopName { get; }
    public ObservableCollection<CatalogNode> Children { get; } = [];
    public IReadOnlyList<string> Items { get; } = [];
    public int SlotCount { get; }
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

    public string DisplayName => Type switch
    {
        CatalogNodeType.Shop => $"{Title} ({SlotCount})",
        CatalogNodeType.ShopSegment => $"{Title} ({Items.Count})",
        _ => Title
    };

    public bool CanMoveShop => Type is CatalogNodeType.Shop or CatalogNodeType.ShopSegment;

    public event PropertyChangedEventHandler? PropertyChanged;
}
