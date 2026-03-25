namespace MerchantsPlus.Generator;

public sealed class MainWindowFactory
{
    public MainWindow Create()
    {
        return new MainWindow
        {
            DataContext = CatalogViewModel.FromCatalog(ItemCatalog.Categories, ItemCatalog.Unorganized)
        };
    }
}