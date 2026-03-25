namespace MerchantsPlus.Generator;

public class CatalogInitializationService{
    private readonly ItemCatalogState _state;
    private readonly CatalogStorageGateway _storage;
    private readonly CatalogConfigurationSupport _configurationSupport;

    public CatalogInitializationService(
        ItemCatalogState state,
        CatalogStorageGateway storage,
        CatalogConfigurationSupport configurationSupport)
    {
        _state = state;
        _storage = storage;
        _configurationSupport = configurationSupport;
    }

    public void Initialize()
    {
        ItemCatalogConfiguration.LoadOrCreateDefaultConfiguration(_state.MutableCategories);

        var unorganizedBlacklistConfig = _storage.LoadUnorganizedBlacklistConfig();
        var globalFlagsConfig = _storage.LoadGlobalFlagsConfig();

        _state.SetUnorganizedBlacklistConfig(unorganizedBlacklistConfig);
        _state.SetGlobalFlagsConfig(globalFlagsConfig);

        _storage.SaveUnorganizedBlacklistConfig(unorganizedBlacklistConfig);
        _storage.SaveGlobalFlagsConfig(globalFlagsConfig);
        _configurationSupport.SaveConfiguration();
    }
}