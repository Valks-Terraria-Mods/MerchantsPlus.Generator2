using System.Text.Json;

namespace MerchantsPlus.Generator;

public static class CatalogStorage
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private static string CategoriesDirectory => Path.Combine(Environment.CurrentDirectory, "catalog-data", "categories");
    private static string MerchantsDirectory => Path.Combine(Environment.CurrentDirectory, "catalog-data", "merchants");
    private static string OutputDirectory => Path.Combine(Environment.CurrentDirectory, "catalog-data", "output");
    private static string PricingDirectory => Path.Combine(Environment.CurrentDirectory, "catalog-data", "pricing");
    private static string SettingsDirectory => Path.Combine(Environment.CurrentDirectory, "catalog-data", "settings");
    private static string UnorganizedDirectory => Path.Combine(Environment.CurrentDirectory, "catalog-data", "unorganized");

    public static List<CategoryConfig> LoadCategoryConfigs() => CategoryStorage.LoadCategoryConfigs(CategoriesDirectory, JsonOptions);

    public static void SaveCategoryConfigs(IReadOnlyList<CategoryConfig> configs) => CategoryStorage.SaveCategoryConfigs(CategoriesDirectory, configs, JsonOptions);

    public static void SaveCatalogOutput(IReadOnlyList<Category> categories) => CategoryStorage.SaveCatalogOutput(OutputDirectory, categories, JsonOptions);

    public static List<MerchantAssignmentConfig> LoadMerchantAssignmentConfigs() => MerchantStorage.LoadMerchantAssignmentConfigs(MerchantsDirectory, JsonOptions);

    public static void SaveMerchantAssignmentConfigs(IReadOnlyList<MerchantAssignmentConfig> configs) => MerchantStorage.SaveMerchantAssignmentConfigs(MerchantsDirectory, configs, JsonOptions);

    public static void SaveMerchantOutputs(IReadOnlyList<MerchantOutput> merchants) => MerchantStorage.SaveMerchantOutputs(OutputDirectory, merchants, JsonOptions);

    public static IReadOnlyList<string> ListOutputFiles()
    {
        Directory.CreateDirectory(OutputDirectory);
        return Directory.EnumerateFiles(OutputDirectory, "*.json")
            .Select(Path.GetFileName)
            .Select(name => name ?? string.Empty)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    public static string GetOutputDirectory()
    {
        Directory.CreateDirectory(OutputDirectory);
        return OutputDirectory;
    }

    public static string GetOutputFilePath(string fileName)
    {
        Directory.CreateDirectory(OutputDirectory);
        var safeName = Path.GetFileName(fileName);
        return string.IsNullOrWhiteSpace(safeName) ? string.Empty : Path.Combine(OutputDirectory, safeName);
    }

    public static string LoadOutputFile(string fileName)
    {
        var path = GetOutputFilePath(fileName);
        if (string.IsNullOrWhiteSpace(path))
            return string.Empty;

        return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
    }

    public static Dictionary<string, int> LoadItemPriceOverrides() => StoragePricingComponent.LoadItemPriceOverrides(PricingDirectory, JsonOptions);

    public static void SaveItemPriceOverrides(IReadOnlyDictionary<string, int> overrides) => StoragePricingComponent.SaveItemPriceOverrides(PricingDirectory, overrides, JsonOptions);

    public static int LoadUnassignedDefaultPrice() => StoragePricingComponent.LoadUnassignedDefaultPrice(PricingDirectory, JsonOptions);

    public static void SaveUnassignedDefaultPrice(int copper) => StoragePricingComponent.SaveUnassignedDefaultPrice(PricingDirectory, copper, JsonOptions);

    public static Dictionary<string, string[]> LoadItemConditions() => StoragePricingComponent.LoadItemConditions(PricingDirectory, JsonOptions);

    public static void SaveItemConditions(IReadOnlyDictionary<string, string[]> itemConditions) => StoragePricingComponent.SaveItemConditions(PricingDirectory, itemConditions, JsonOptions);

    public static GlobalFlagsConfig LoadGlobalFlagsConfig() => StoragePricingComponent.LoadGlobalFlagsConfig(PricingDirectory, JsonOptions);

    public static void SaveGlobalFlagsConfig(GlobalFlagsConfig config) => StoragePricingComponent.SaveGlobalFlagsConfig(PricingDirectory, config, JsonOptions);

    public static UnorganizedBlacklistConfig LoadUnorganizedBlacklistConfig() => StoragePricingComponent.LoadUnorganizedBlacklistConfig(PricingDirectory, JsonOptions);

    public static void SaveUnorganizedBlacklistConfig(UnorganizedBlacklistConfig config) => StoragePricingComponent.SaveUnorganizedBlacklistConfig(PricingDirectory, config, JsonOptions);

    public static SettingsConfig LoadSettingsConfig() => StorageSettingsComponent.LoadSettingsConfig(SettingsDirectory, JsonOptions);

    public static void SaveSettingsConfig(SettingsConfig config) => StorageSettingsComponent.SaveSettingsConfig(SettingsDirectory, config, JsonOptions);

    public static UnorganizedCache LoadUnorganizedCache() => StorageUnorganizedComponent.LoadUnorganizedCache(UnorganizedDirectory, JsonOptions);

    public static void SaveUnorganizedCache(UnorganizedCache cache) => StorageUnorganizedComponent.SaveUnorganizedCache(UnorganizedDirectory, cache, JsonOptions);
}
