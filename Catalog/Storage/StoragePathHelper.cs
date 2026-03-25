namespace MerchantsPlus.Generator;

internal static class StoragePathHelper
{
    internal static string NextAvailableSlug(string name, HashSet<string> usedNames)
    {
        var baseName = Slugify(name);
        var fileName = baseName;
        var suffix = 2;

        while (!usedNames.Add(fileName))
        {
            fileName = $"{baseName}-{suffix}";
            suffix++;
        }

        return fileName;
    }

    internal static string ConvertToFourSpaceIndent(string json)
    {
        return json;
    }

    private static string Slugify(string name)
    {
        var chars = name
            .Trim()
            .ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '-')
            .ToArray();

        var slug = new string(chars);
        while (slug.Contains("--", StringComparison.Ordinal))
            slug = slug.Replace("--", "-", StringComparison.Ordinal);

        slug = slug.Trim('-');
        return string.IsNullOrWhiteSpace(slug) ? "category" : slug;
    }
}
