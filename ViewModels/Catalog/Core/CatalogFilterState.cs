using System.Collections.ObjectModel;

namespace MerchantsPlus.Generator;

public abstract class CatalogFilterState : CatalogState
{
    private string _catalogTreeFilterText = string.Empty;
    private string _merchantFilterText = string.Empty;

    public ObservableCollection<CatalogNode> FilteredRootNodes { get; } = [];
    public ObservableCollection<string> FilteredMerchantNpcIds { get; } = [];

    public string CatalogTreeFilterText
    {
        get => _catalogTreeFilterText;
        set
        {
            if (_catalogTreeFilterText == value)
                return;

            _catalogTreeFilterText = value;
            OnPropertyChanged();
            RefreshFilteredRootNodes();
        }
    }

    public string MerchantFilterText
    {
        get => _merchantFilterText;
        set
        {
            if (_merchantFilterText == value)
                return;

            _merchantFilterText = value;
            OnPropertyChanged();
            RefreshFilteredMerchantNpcIds();
        }
    }

    protected void RefreshFilteredRootNodes()
    {
        FilteredRootNodes.Clear();

        foreach (var node in RootNodes)
        {
            if (string.IsNullOrWhiteSpace(_catalogTreeFilterText) || NodeMatchesFilter(node, _catalogTreeFilterText))
                FilteredRootNodes.Add(node);
        }

        if (_selectedNode is null || IsNodeVisible(_selectedNode))
            return;

        _selectedNode = null;
        SelectedNode = GetFirstVisibleNode();
    }

    protected void RefreshFilteredMerchantNpcIds()
    {
        FilteredMerchantNpcIds.Clear();

        foreach (var npc in MerchantNpcIds)
        {
            if (string.IsNullOrWhiteSpace(_merchantFilterText) || MatchesMerchantFilter(npc, _merchantFilterText))
                FilteredMerchantNpcIds.Add(npc);
        }

        OnMerchantFilterChanged();
    }

    protected virtual bool MatchesMerchantFilter(string npc, string filter)
    {
        return npc.Contains(filter, StringComparison.OrdinalIgnoreCase);
    }

    private bool IsNodeVisible(CatalogNode target)
    {
        foreach (var node in FilteredRootNodes)
        {
            if (ContainsNodeReference(node, target))
                return true;
        }

        return false;
    }

    private static bool NodeMatchesFilter(CatalogNode node, string filter)
    {
        if (node.DisplayName.Contains(filter, StringComparison.OrdinalIgnoreCase))
            return true;

        foreach (var child in node.Children)
        {
            if (NodeMatchesFilter(child, filter))
                return true;
        }

        return false;
    }

    private static bool ContainsNodeReference(CatalogNode root, CatalogNode target)
    {
        if (ReferenceEquals(root, target))
            return true;

        foreach (var child in root.Children)
        {
            if (ContainsNodeReference(child, target))
                return true;
        }

        return false;
    }

    private CatalogNode? GetFirstVisibleNode()
    {
        if (FilteredRootNodes.Count == 0)
            return null;

        var root = FilteredRootNodes[0];
        return root.Children.FirstOrDefault() ?? root;
    }
}
