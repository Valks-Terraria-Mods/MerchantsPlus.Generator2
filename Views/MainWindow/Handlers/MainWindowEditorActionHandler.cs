using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MerchantsPlus.Generator;

public sealed class MainWindowEditorActionHandler
{
    private readonly CatalogViewModelAccessor _viewModelAccessor;
    private readonly CatalogInteractionService _catalogInteractionService;
    private readonly GlobalFlagsActionHandler _globalFlagsActionHandler;

    public MainWindowEditorActionHandler(
        CatalogViewModelAccessor viewModelAccessor,
        CatalogInteractionService catalogInteractionService,
        GlobalFlagsActionHandler globalFlagsActionHandler)
    {
        _viewModelAccessor = viewModelAccessor;
        _catalogInteractionService = catalogInteractionService;
        _globalFlagsActionHandler = globalFlagsActionHandler;
    }

    public Task AddCategoryAsync() => _catalogInteractionService.AddCategoryAsync();

    public void UpdateCategory() => RunWithItemViewer(vm => vm.UpdateSelectedCategory());

    public void RemoveCategory() => RunWithItemViewer(vm => vm.RemoveSelectedCategory());

    public void SaveShop() => RunWithItemViewer(vm => vm.SaveShop());

    public Task NewShopAsync() => _catalogInteractionService.NewShopAsync();

    public void RemoveShop() => RunWithItemViewer(vm => vm.RemoveSelectedShop());

    public void ItemRowDoubleTapped(object? sender) => _catalogInteractionService.HandleItemRowDoubleTapped(sender);

    public void ExcludeItem(object? sender) => _catalogInteractionService.HandleExcludeItem(sender);

    public void CatalogTreeContextRequested(ContextRequestedEventArgs e) => _catalogInteractionService.HandleCatalogTreeContextRequested(e);

    public void AddGlobalWhitelistRule() => _globalFlagsActionHandler.AddWhitelistRule();

    public void RemoveGlobalWhitelistRule(object? sender) => _globalFlagsActionHandler.RemoveWhitelistRule(sender);

    public void AddGlobalBlacklistRule() => _globalFlagsActionHandler.AddBlacklistRule();

    public void RemoveGlobalBlacklistRule(object? sender) => _globalFlagsActionHandler.RemoveBlacklistRule(sender);

    public void ApplyGlobalFlags() => _globalFlagsActionHandler.Apply();

    public void ResetAllPrices() => _globalFlagsActionHandler.ResetAllPrices();

    public void ResetAllConditions() => _globalFlagsActionHandler.ResetAllConditions();

    public void SaveUnassignedDefaultPrice() => _globalFlagsActionHandler.SaveUnassignedDefaultPrice();

    public void ResetUnassignedDefaultPrice() => _globalFlagsActionHandler.ResetUnassignedDefaultPrice();

    public void ResetSelectedItemPriceOverride() => _viewModelAccessor.Execute(vm => vm.ResetSelectedItemPriceOverride());

    public void ResetSelectedShopPriceOverrides() => _viewModelAccessor.Execute(vm => vm.ResetSelectedShopPriceOverrides());

    public void ResetSelectedCategoryPriceOverrides() => _viewModelAccessor.Execute(vm => vm.ResetSelectedCategoryPriceOverrides());

    public void ResetSelectedItemConditionOverride() => _viewModelAccessor.Execute(vm => vm.ResetSelectedItemConditionOverride());

    public void ResetSelectedShopConditionOverrides() => _viewModelAccessor.Execute(vm => vm.ResetSelectedShopConditionOverrides());

    public void ResetSelectedCategoryConditionOverrides() => _viewModelAccessor.Execute(vm => vm.ResetSelectedCategoryConditionOverrides());

    public void SaveUnorganizedBlacklist() => _viewModelAccessor.Execute(vm => vm.SaveUnorganizedBlacklistSettings());

    public void AddCategoryRule() => _viewModelAccessor.Execute(vm => vm.AddCategoryKeywordRule());

    public void RemoveCategoryRule(object? sender) => RemoveRule(sender, (vm, row) => vm.RemoveCategoryKeywordRule(row));

    public void AddShopRule() => _viewModelAccessor.Execute(vm => vm.AddShopKeywordRule());

    public void RemoveShopRule(object? sender) => RemoveRule(sender, (vm, row) => vm.RemoveShopKeywordRule(row));

    public void AddUnorganizedRule() => _viewModelAccessor.Execute(vm => vm.AddUnorganizedKeywordRule());

    public void RemoveUnorganizedRule(object? sender) => RemoveRule(sender, (vm, row) => vm.RemoveUnorganizedKeywordRule(row));

    public void ClearUnorganizedBlacklist() => _viewModelAccessor.Execute(vm => vm.ClearUnorganizedBlacklistSettings());

    private void RunWithItemViewer(Action<CatalogViewModel> action)
    {
        _viewModelAccessor.Execute(vm =>
        {
            vm.ShowItemViewerInCenter();
            action(vm);
        });
    }

    private void RemoveRule(object? sender, Action<CatalogViewModel, KeywordRuleEditorRow> action)
    {
        if (!KeywordRuleRowResolver.TryResolve(sender, out var row))
            return;

        _viewModelAccessor.Execute(vm => action(vm, row));
    }
}