using System.Diagnostics;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;

namespace MerchantsPlus.Generator;

public sealed class CatalogInteractionService{
    private static readonly Regex CamelBoundary = new("([a-z0-9])([A-Z])", RegexOptions.Compiled);

    private readonly Func<CatalogViewModel?> _viewModelAccessor;
    private readonly MainWindowDialogService _dialogService;

    public CatalogInteractionService(Func<CatalogViewModel?> viewModelAccessor, MainWindowDialogService dialogService)
    {
        _viewModelAccessor = viewModelAccessor;
        _dialogService = dialogService;
    }

    public async Task AddCategoryAsync()
    {
        var vm = _viewModelAccessor();
        if (vm is null)
            return;

        vm.ShowItemViewerInCenter();
        var name = await _dialogService.PromptForNameAsync("New Category", "Enter category name:");
        if (string.IsNullOrWhiteSpace(name))
            return;

        vm.NewCategoryName = name;
        vm.AddCategory();
    }

    public async Task NewShopAsync()
    {
        var vm = _viewModelAccessor();
        if (vm is null)
            return;

        vm.ShowItemViewerInCenter();
        var name = await _dialogService.PromptForNameAsync("New Shop", "Enter shop name:");
        if (string.IsNullOrWhiteSpace(name))
            return;

        vm.BeginNewShop();
        vm.NewShopName = name;
    }

    public void HandleItemRowDoubleTapped(object? sender)
    {
        if (sender is not Control { DataContext: CatalogItemRow row } || row.IsShopSlot)
            return;

        var title = CamelBoundary.Replace(row.Name.Replace("_", " "), "$1 $2").Trim();
        var url = $"https://terraria.wiki.gg/wiki/{Uri.EscapeDataString(title)}";

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = url,
                UseShellExecute = true
            });
        }
        catch
        {
            // Ignore launcher failures for unsupported desktop environments.
        }
    }

    public void HandleExcludeItem(object? sender)
    {
        var vm = _viewModelAccessor();
        if (vm is null)
            return;

        if (sender is not Control { DataContext: CatalogItemRow row } || row.IsShopSlot)
            return;

        vm.ExcludeItem(row.Name);
    }

    public void HandleCatalogTreeContextRequested(ContextRequestedEventArgs e)
    {
        var vm = _viewModelAccessor();
        if (vm is null)
            return;

        if (e.Source is not Visual visual)
            return;

        var treeItem = visual.GetSelfAndVisualAncestors().OfType<TreeViewItem>().FirstOrDefault();
        if (treeItem?.DataContext is not CatalogNode node || !node.CanMoveShop)
            return;

        vm.ShowItemViewerInCenter();
        vm.SelectedNode = node;

        var treeScrollViewer = treeItem
            .GetSelfAndVisualAncestors()
            .OfType<ScrollViewer>()
            .FirstOrDefault();
        var scrollOffset = treeScrollViewer?.Offset ?? default;

        var moveToCategoryItem = new MenuItem
        {
            Header = "Move Shop To Category...",
            DataContext = node
        };
        moveToCategoryItem.Click += async (_, _) => await MoveShopAsync(node, treeScrollViewer, scrollOffset, moveToShopDestination: false);

        var moveToShopItem = new MenuItem
        {
            Header = "Move Shop To Shop...",
            DataContext = node
        };
        moveToShopItem.Click += async (_, _) => await MoveShopAsync(node, treeScrollViewer, scrollOffset, moveToShopDestination: true);

        var menu = new ContextMenu { ItemsSource = new[] { moveToCategoryItem, moveToShopItem } };
        menu.Open(treeItem);
        e.Handled = true;
    }

    private async Task MoveShopAsync(CatalogNode node, ScrollViewer? treeScrollViewer, Vector scrollOffset, bool moveToShopDestination)
    {
        var vm = _viewModelAccessor();
        if (vm is null || !node.CanMoveShop)
            return;

        vm.ShowItemViewerInCenter();
        vm.SelectedNode = node;

        var sourceCategory = vm.SelectedCategoryForShop;
        var shopName = node.EditorShopName;
        if (string.IsNullOrWhiteSpace(sourceCategory) || string.IsNullOrWhiteSpace(shopName))
            return;

        var categoryDestinations = ItemCatalog.Categories
            .OrderBy(category => category.Name, StringComparer.OrdinalIgnoreCase)
            .Select(category => new ShopMoveDestination
            {
                Label = $"Category: {category.Name}",
                CategoryName = category.Name,
                ParentShopName = string.Empty
            })
            .ToList();

        var shopDestinations = ItemCatalog.Categories
            .OrderBy(category => category.Name, StringComparer.OrdinalIgnoreCase)
            .SelectMany(category => category.Shops
                .OrderBy(shop => shop.Name, StringComparer.OrdinalIgnoreCase)
                .Where(candidateShop =>
                    !string.Equals(category.Name, sourceCategory, StringComparison.OrdinalIgnoreCase)
                    || !string.Equals(candidateShop.Name, shopName, StringComparison.OrdinalIgnoreCase))
                .Select(candidateShop => new ShopMoveDestination
                {
                    Label = $"Shop: {category.Name} / {candidateShop.Name}",
                    CategoryName = category.Name,
                    ParentShopName = candidateShop.Name
                }))
            .ToList();

        var destinations = moveToShopDestination ? shopDestinations : categoryDestinations;
        if (destinations.Count == 0)
        {
            vm.EditorStatus = moveToShopDestination
                ? "No valid shop destinations available for this shop."
                : "No valid category destinations available for this shop.";
            return;
        }

        var destination = await _dialogService.PromptForShopMoveDestinationAsync(
            moveToShopDestination ? "Move Shop To Shop" : "Move Shop To Category",
            $"Move '{shopName}' to:",
            moveToShopDestination ? "Select target shop:" : "Select target category:",
            destinations);

        if (destination is not null)
        {
            vm.MoveSelectedShop(destination.CategoryName, destination.ParentShopName);

            if (treeScrollViewer is not null)
            {
                Dispatcher.UIThread.Post(
                    () => treeScrollViewer.Offset = scrollOffset,
                    DispatcherPriority.Background);
            }
        }
    }
}
