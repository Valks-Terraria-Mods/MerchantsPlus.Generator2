using Avalonia.Controls;

namespace MerchantsPlus.Generator;

public sealed class ApplicationStartupCoordinator
{
    private readonly CatalogDataBootstrapper _catalogDataBootstrapper;
    private readonly MainWindowFactory _mainWindowFactory;

    public ApplicationStartupCoordinator(
        CatalogDataBootstrapper catalogDataBootstrapper,
        MainWindowFactory mainWindowFactory)
    {
        _catalogDataBootstrapper = catalogDataBootstrapper;
        _mainWindowFactory = mainWindowFactory;
    }

    public Window CreateMainWindow()
    {
        _catalogDataBootstrapper.BuildCatalog();
        return _mainWindowFactory.Create();
    }
}