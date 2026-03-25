using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MerchantsPlus.Generator;

public partial class GlobalFlagsWindow : Window
{
    private readonly GlobalFlagsActionHandler _globalFlagsActionHandler;
    private readonly Action _closeWindow;

    public GlobalFlagsWindow()
    {
        InitializeComponent();

        var viewModelAccessor = new CatalogViewModelAccessor(() => DataContext);
        _globalFlagsActionHandler = new GlobalFlagsActionHandler(viewModelAccessor);
        _closeWindow = Close;
    }

    private void OnAddGlobalWhitelistRuleClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.AddWhitelistRule();

    private void OnRemoveGlobalWhitelistRuleClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.RemoveWhitelistRule(sender);

    private void OnAddGlobalBlacklistRuleClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.AddBlacklistRule();

    private void OnRemoveGlobalBlacklistRuleClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.RemoveBlacklistRule(sender);

    private void OnApplyGlobalFlagsClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.Apply();

    private void OnResetAllPricesClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.ResetAllPrices();

    private void OnResetAllConditionsClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.ResetAllConditions();

    private void OnSaveUnassignedDefaultPriceClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.SaveUnassignedDefaultPrice();

    private void OnResetUnassignedDefaultPriceClick(object? sender, RoutedEventArgs e) => _globalFlagsActionHandler.ResetUnassignedDefaultPrice();

    private void OnCloseClick(object? sender, RoutedEventArgs e) => _closeWindow();
}
