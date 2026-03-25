using System.Text.Json;

namespace MerchantsPlus.Generator;

internal static class MerchantStorage
{
    internal static List<MerchantAssignmentConfig> LoadMerchantAssignmentConfigs(string merchantsDirectory, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(merchantsDirectory);
        var path = Path.Combine(merchantsDirectory, "assignments.json");

        if (!File.Exists(path))
            return [];

        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<MerchantAssignmentConfig>>(json, options) ?? [];
    }

    internal static void SaveMerchantAssignmentConfigs(string merchantsDirectory, IReadOnlyList<MerchantAssignmentConfig> configs, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(merchantsDirectory);
        var path = Path.Combine(merchantsDirectory, "assignments.json");
        var json = JsonSerializer.Serialize(configs, options);
        File.WriteAllText(path, StoragePathHelper.ConvertToFourSpaceIndent(json));
    }

    internal static void SaveMerchantOutputs(string outputDirectory, IReadOnlyList<MerchantOutput> merchants, JsonSerializerOptions options)
    {
        Directory.CreateDirectory(outputDirectory);

        foreach (var file in Directory.GetFiles(outputDirectory, "*.json", SearchOption.TopDirectoryOnly))
            File.Delete(file);

        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var merchant in merchants)
        {
            var fileName = StoragePathHelper.NextAvailableSlug(merchant.Name, usedNames);
            var path = Path.Combine(outputDirectory, $"{fileName}.json");
            var json = JsonSerializer.Serialize(merchant, options);
            File.WriteAllText(path, StoragePathHelper.ConvertToFourSpaceIndent(json));
        }
    }
}
