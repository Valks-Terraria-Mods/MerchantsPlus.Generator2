using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace MerchantsPlus.Generator;

public partial class MainWindow : Window
{
    private readonly CatalogViewModelAccessor _viewModelAccessor;
    private readonly OutputPreviewController _outputPreviewController;
    private readonly MainWindowLifecycleCoordinator _lifecycleCoordinator;
    private readonly MainWindowOverlayController _overlayController;
    private readonly MainWindowEditorActionHandler _editorActionHandler;
    private readonly MainWindowShellActionHandler _shellActionHandler;

    public MainWindow()
    {
        InitializeComponent();

        _viewModelAccessor = new CatalogViewModelAccessor(() => DataContext);

        _outputPreviewController = new OutputPreviewController(
            () => _viewModelAccessor.Current,
            () => this.FindControl<SelectableTextBlock>("OutputJsonPreview"),
            () => this.FindControl<ScrollViewer>("OutputJsonScrollViewer"),
            new JsonSegmentBuilder(),
            new OutputPreviewCache(limit: 5),
            new OutputSearchCoordinator(),
            new OutputInlineRenderer());

        _lifecycleCoordinator = new MainWindowLifecycleCoordinator(this, _viewModelAccessor, _outputPreviewController);
        _overlayController = new MainWindowOverlayController(this);

        var globalFlagsActionHandler = new GlobalFlagsActionHandler(_viewModelAccessor);
        var catalogInteractionService = new CatalogInteractionService(
            () => _viewModelAccessor.Current,
            new MainWindowDialogService(this));
        _editorActionHandler = new MainWindowEditorActionHandler(
            _viewModelAccessor,
            catalogInteractionService,
            globalFlagsActionHandler);
        _shellActionHandler = new MainWindowShellActionHandler(
            _viewModelAccessor,
            _overlayController,
            _outputPreviewController,
            Close);

        DataContextChanged += OnMainWindowDataContextChanged;
    }

    protected override void OnClosed(EventArgs e)
    {
        DataContextChanged -= OnMainWindowDataContextChanged;
        _lifecycleCoordinator.Dispose();
        _outputPreviewController.Dispose();
        base.OnClosed(e);
    }

    private void OnMainWindowDataContextChanged(object? sender, EventArgs e) => _lifecycleCoordinator.HandleDataContextChanged();

    private async void OnAddCategoryClick(object? sender, RoutedEventArgs e) => await _editorActionHandler.AddCategoryAsync();
    private void OnUpdateCategoryClick(object? sender, RoutedEventArgs e) => _editorActionHandler.UpdateCategory();
    private void OnRemoveCategoryClick(object? sender, RoutedEventArgs e) => _editorActionHandler.RemoveCategory();
    private void OnSaveShopClick(object? sender, RoutedEventArgs e) => _editorActionHandler.SaveShop();
    private async void OnNewShopClick(object? sender, RoutedEventArgs e) => await _editorActionHandler.NewShopAsync();
    private void OnRemoveShopClick(object? sender, RoutedEventArgs e) => _editorActionHandler.RemoveShop();
    private void OnItemRowDoubleTapped(object? sender, TappedEventArgs e) => _editorActionHandler.ItemRowDoubleTapped(e.Source ?? sender);
    private void OnExcludeItemClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ExcludeItem(sender);
    private void OnCatalogTreeContextRequested(object? sender, ContextRequestedEventArgs e) => _editorActionHandler.CatalogTreeContextRequested(e);

    private void OnForceRefreshClick(object? sender, RoutedEventArgs e) => _shellActionHandler.ForceRefresh();
    private void OnExitClick(object? sender, RoutedEventArgs e) => _shellActionHandler.Exit();
    private void OnOpenOutputFolderClick(object? sender, RoutedEventArgs e) => _shellActionHandler.OpenOutputFolder();
    private void OnGenerateCatalogScriptClick(object? sender, RoutedEventArgs e) => _shellActionHandler.GenerateCatalogScript();
    private void OnOutputSearchPrevClick(object? sender, RoutedEventArgs e) => _shellActionHandler.OutputSearchPrevious();
    private void OnOutputSearchNextClick(object? sender, RoutedEventArgs e) => _shellActionHandler.OutputSearchNext();

    private void OnOpenSettingsClick(object? sender, RoutedEventArgs e) => _shellActionHandler.OpenSettings();
    private void OnCloseSettingsPopupClick(object? sender, RoutedEventArgs e) => _shellActionHandler.CloseSettings();
    private void OnSettingsOverlayPointerPressed(object? sender, PointerPressedEventArgs e) => _shellActionHandler.CloseSettingsFromBackdrop(e);
    private static void OnSettingsPanelPointerPressed(object? sender, PointerPressedEventArgs e) => MainWindowOverlayController.ConsumePanelPointer(e);
    private void OnAutoDetectTModLoaderClick(object? sender, RoutedEventArgs e) => _shellActionHandler.AutoDetectTModLoader();
    private void OnSaveSettingsClick(object? sender, RoutedEventArgs e) => _shellActionHandler.SaveSettings();

    private void OnOpenGlobalFlagsClick(object? sender, RoutedEventArgs e) => _shellActionHandler.OpenGlobalFlags();
    private void OnCloseGlobalFlagsPopupClick(object? sender, RoutedEventArgs e) => _shellActionHandler.CloseGlobalFlags();
    private void OnGlobalFlagsOverlayPointerPressed(object? sender, PointerPressedEventArgs e) => _shellActionHandler.CloseGlobalFlagsFromBackdrop(e);
    private static void OnGlobalFlagsPanelPointerPressed(object? sender, PointerPressedEventArgs e) => MainWindowOverlayController.ConsumePanelPointer(e);

    private void OnAddGlobalWhitelistRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.AddGlobalWhitelistRule();
    private void OnRemoveGlobalWhitelistRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.RemoveGlobalWhitelistRule(sender);

    private void OnAddGlobalBlacklistRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.AddGlobalBlacklistRule();
    private void OnRemoveGlobalBlacklistRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.RemoveGlobalBlacklistRule(sender);

    private void OnApplyGlobalFlagsClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ApplyGlobalFlags();
    private void OnResetAllPricesClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetAllPrices();
    private void OnResetAllConditionsClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetAllConditions();

    private void OnResetSelectedItemPriceOverrideClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetSelectedItemPriceOverride();
    private void OnResetSelectedShopPriceOverridesClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetSelectedShopPriceOverrides();
    private void OnResetSelectedCategoryPriceOverridesClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetSelectedCategoryPriceOverrides();
    private void OnSaveUnassignedDefaultPriceClick(object? sender, RoutedEventArgs e) => _editorActionHandler.SaveUnassignedDefaultPrice();
    private void OnResetUnassignedDefaultPriceClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetUnassignedDefaultPrice();

    private void OnResetSelectedItemConditionOverrideClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetSelectedItemConditionOverride();
    private void OnResetSelectedShopConditionOverridesClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetSelectedShopConditionOverrides();
    private void OnResetSelectedCategoryConditionOverridesClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ResetSelectedCategoryConditionOverrides();

    private void OnSaveUnorganizedBlacklistClick(object? sender, RoutedEventArgs e) => _editorActionHandler.SaveUnorganizedBlacklist();
    private void OnAddCategoryRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.AddCategoryRule();
    private void OnRemoveCategoryRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.RemoveCategoryRule(sender);

    private void OnAddShopRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.AddShopRule();
    private void OnRemoveShopRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.RemoveShopRule(sender);

    private void OnAddUnorganizedRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.AddUnorganizedRule();
    private void OnRemoveUnorganizedRuleClick(object? sender, RoutedEventArgs e) => _editorActionHandler.RemoveUnorganizedRule(sender);

    private void OnClearUnorganizedBlacklistClick(object? sender, RoutedEventArgs e) => _editorActionHandler.ClearUnorganizedBlacklist();
}
