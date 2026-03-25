using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace MerchantsPlus.Generator;

public partial class MerchantWorkspaceView : UserControl
{
    public MerchantWorkspaceView()
    {
        InitializeComponent();
    }

    private void OnAddMerchantShopClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is CatalogViewModel viewModel)
            viewModel.AddSelectedMerchantShop();
    }

    private void OnRemoveMerchantShopClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is CatalogViewModel viewModel && sender is Control { DataContext: MerchantShopBadgeRow row })
            viewModel.RemoveMerchantOwnedShop(row);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}