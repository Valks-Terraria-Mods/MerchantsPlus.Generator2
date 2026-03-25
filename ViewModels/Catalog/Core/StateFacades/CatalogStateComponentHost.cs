using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MerchantsPlus.Generator;

public abstract class CatalogStateComponentHost : INotifyPropertyChanged
{
    private const int DefaultMaxItemsPerShopChunk = 40;
    private readonly CatalogCollectionsComponent _collections;
    private readonly CatalogEditorInputsComponent _editorInputs;
    private readonly CatalogSelectionComponent _selection;
    private readonly CatalogSettingsComponent _settings;
    private readonly CatalogOutputComponent _output;
    private readonly CatalogMetricsComponent _metrics;

    protected CatalogStateComponentHost() : this(new CatalogStateComponents()) { }

    protected CatalogStateComponentHost(CatalogStateComponents components)
    {
        ArgumentNullException.ThrowIfNull(components);
        _collections = components.Collections;
        _editorInputs = components.EditorInputs;
        _selection = components.Selection;
        _settings = components.Settings;
        _output = components.Output;
        _metrics = components.Metrics;
    }

    protected int MaxItemsPerShopChunk => DefaultMaxItemsPerShopChunk;
    protected CatalogCollectionsComponent Collections => _collections;
    protected CatalogEditorInputsComponent EditorInputs => _editorInputs;
    protected CatalogSelectionComponent Selection => _selection;
    protected CatalogSettingsComponent Settings => _settings;
    protected CatalogOutputComponent Output => _output;
    protected CatalogMetricsComponent Metrics => _metrics;

    protected CatalogNode? _selectedNode { get => _selection.SelectedNode; set => _selection.SelectedNode = value; }
    protected string _filterText { get => _selection.FilterText; set => _selection.FilterText = value; }
    protected bool _isSettingsLoading { get => _settings.IsSettingsLoading; set => _settings.IsSettingsLoading = value; }

    public ObservableCollection<CatalogNode> RootNodes => _collections.RootNodes;
    public ObservableCollection<CatalogItemRow> VisibleItems => _collections.VisibleItems;
    public ObservableCollection<CatalogItemRow> SecondaryVisibleItems => _collections.SecondaryVisibleItems;
    public ObservableCollection<MerchantTreeNode> VisibleMerchantAssignments => _collections.VisibleMerchantAssignments;
    public ObservableCollection<MerchantTreeNode> MerchantWorkspaceNodes => _collections.MerchantWorkspaceNodes;
    public ObservableCollection<MerchantShopBadgeRow> MerchantOwnedShopBadges => _collections.MerchantOwnedShopBadges;
    public ObservableCollection<MerchantShopBadgeRow> MerchantAvailableShops => _collections.MerchantAvailableShops;
    public ObservableCollection<string> MerchantNpcIds => _collections.MerchantNpcIds;
    public ObservableCollection<string> OutputFiles => _collections.OutputFiles;
    public ObservableCollection<AssignmentTreeNode> MerchantAssignmentOptionsTree => _collections.MerchantAssignmentOptionsTree;
    public ObservableCollection<AssignmentTreeNode> MerchantAssignmentAvailableTree => _collections.MerchantAssignmentAvailableTree;
    public ObservableCollection<AssignmentTreeNode> MerchantAssignmentAssignedTree => _collections.MerchantAssignmentAssignedTree;
    public ObservableCollection<SelectableOption> MerchantCategoryOptions => _collections.MerchantCategoryOptions;
    public ObservableCollection<SelectableOption> MerchantShopOptions => _collections.MerchantShopOptions;
    public ObservableCollection<string> CategoryOptions => _collections.CategoryOptions;
    public ObservableCollection<string> ShopOptions => _collections.ShopOptions;
    public ObservableCollection<KeywordRuleEditorRow> CategoryKeywordRules => _collections.CategoryKeywordRules;
    public ObservableCollection<KeywordRuleEditorRow> ShopKeywordRules => _collections.ShopKeywordRules;
    public ObservableCollection<KeywordRuleEditorRow> UnorganizedKeywordRules => _collections.UnorganizedKeywordRules;
    public ObservableCollection<string> BlacklistedUnorganizedItems => _collections.BlacklistedUnorganizedItems;
    public ObservableCollection<string> FilteredBlacklistedUnorganizedItems => _collections.FilteredBlacklistedUnorganizedItems;
    public IReadOnlyList<string> RuleModes => _collections.RuleModes;
    public IReadOnlyList<string> RuleMatches => _collections.RuleMatches;
    public IReadOnlyList<string> MerchantSortModes => _collections.MerchantSortModes;
    public IReadOnlyList<string> AppThemeOptions => SettingsDefaults.AppThemes;

    public string AppTheme
    {
        get => Settings.AppTheme;
        set
        {
            var normalized = SettingsDefaults.NormalizeTheme(value);
            if (!SetComponentField(Settings.AppTheme, normalized, v => Settings.AppTheme = v))
                return;
            OnSettingsChanged();
        }
    }

    public double FontSize11 => 11d * Settings.AppTextScale;
    public double FontSize13 => 13d * Settings.AppTextScale;
    public double FontSize16 => 16d * Settings.AppTextScale;
    public double FontSize18 => 18d * Settings.AppTextScale;
    public double FontSize20 => 20d * Settings.AppTextScale;
    public double FontSize22 => 22d * Settings.AppTextScale;
    public double FontSize24 => 24d * Settings.AppTextScale;

    public int CategoryCount { get => _metrics.CategoryCount; protected set => SetComponentField(_metrics.CategoryCount, value, v => _metrics.CategoryCount = v); }
    public int ShopCount { get => _metrics.ShopCount; protected set => SetComponentField(_metrics.ShopCount, value, v => _metrics.ShopCount = v); }
    public int ItemCount { get => _metrics.ItemCount; protected set => SetComponentField(_metrics.ItemCount, value, v => _metrics.ItemCount = v); }
    public string HeaderStats => $"Categories: {CategoryCount}   Shops: {ShopCount}   Items: {ItemCount}";

    protected bool SetComponentField<T>(T current, T value, Action<T> setter, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(current, value))
            return false;

        setter(value);
        OnPropertyChanged(propertyName);
        return true;
    }

    protected void SetField(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
            return;

        field = value;
        OnPropertyChanged(propertyName);
    }

    protected void NotifyAppFontSizesChanged()
    {
        OnPropertyChanged(nameof(FontSize11));
        OnPropertyChanged(nameof(FontSize13));
        OnPropertyChanged(nameof(FontSize16));
        OnPropertyChanged(nameof(FontSize18));
        OnPropertyChanged(nameof(FontSize20));
        OnPropertyChanged(nameof(FontSize22));
        OnPropertyChanged(nameof(FontSize24));
    }

    protected abstract void RefreshVisibleItems();
    protected abstract void OnSelectedCategoryChanged();
    protected abstract void OnSelectedShopChanged();
    protected abstract void OnSelectedNodeChanged();
    protected virtual void OnSelectedMerchantChanged() { }
    protected virtual void OnMerchantFilterChanged() { }
    protected virtual void OnMerchantSortChanged() { }
    protected virtual void OnCatalogLoaded() { }
    protected virtual void OnSettingsChanged() { }
    protected virtual void OnSelectedOutputFileChanged() { }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
