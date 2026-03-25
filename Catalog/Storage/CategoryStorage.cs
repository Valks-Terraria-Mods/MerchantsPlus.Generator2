using System.Text.Json;

namespace MerchantsPlus.Generator;

internal static class CategoryStorage
{
    internal static List<CategoryConfig> LoadCategoryConfigs(string categoriesDirectory, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(categoriesDirectory);

        var files = Directory.GetFiles(categoriesDirectory, "*.json", SearchOption.TopDirectoryOnly);
        var results = new List<CategoryConfig>();

        foreach (var file in files.OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
        {
            var json = File.ReadAllText(file);
            var config = JsonSerializer.Deserialize<CategoryConfig>(json, options);
            if (config is not null)
                results.Add(config);
        }

        return results;
    }

    internal static void SaveCategoryConfigs(string categoriesDirectory, IReadOnlyList<CategoryConfig> configs, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(categoriesDirectory);

        foreach (var file in Directory.GetFiles(categoriesDirectory, "*.json", SearchOption.TopDirectoryOnly))
            File.Delete(file);

        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var config in configs)
        {
            var fileName = StoragePathHelper.NextAvailableSlug(config.Name, usedNames);
            var path = Path.Combine(categoriesDirectory, $"{fileName}.json");
            var json = JsonSerializer.Serialize(config, options);
            File.WriteAllText(path, StoragePathHelper.ConvertToFourSpaceIndent(json));
        }
    }

    internal static void SaveCatalogOutput(string outputDirectory, IReadOnlyList<Category> categories, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(outputDirectory);

        foreach (var file in Directory.GetFiles(outputDirectory, "*.json", SearchOption.TopDirectoryOnly))
            File.Delete(file);

        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var category in categories)
        {
            var fileName = StoragePathHelper.NextAvailableSlug(category.Name, usedNames);
            var output = new CategoryOutput
            {
                Name = category.Name,
                Shops =
                [
                    .. StorageShopOutputBuilder.BuildTopLevelShopOutputs(
                        category,
                        category.Shops)
                ]
            };

            var path = Path.Combine(outputDirectory, $"{fileName}.json");
            var json = JsonSerializer.Serialize(output, options);
            File.WriteAllText(path, StoragePathHelper.ConvertToFourSpaceIndent(json));
        }
    }
}
