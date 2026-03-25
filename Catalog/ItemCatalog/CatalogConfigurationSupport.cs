namespace MerchantsPlus.Generator;

public class CatalogConfigurationSupport{
    private readonly ItemCatalogState _state;
    private readonly CatalogStorageGateway _storage;

    public CatalogConfigurationSupport(ItemCatalogState state, CatalogStorageGateway storage)
    {
        _state = state;
        _storage = storage;
    }

    public void SaveConfiguration()
    {
        var categoryConfigs = ItemCatalogConfiguration.ToConfigs(_state.Categories);
        _storage.SaveCategoryConfigs(categoryConfigs);
    }

    public void ApplyCategoryAndSharedOrderLinks(Category category)
    {
        ItemCatalogConfiguration.ApplyCategoryAndSharedOrderLinks(category);
    }

    public void UpdateDependentSourceNames(Category category, string oldName, string newName)
    {
        ItemCatalogConfiguration.UpdateDependentSourceNames(category, oldName, newName);
    }
}