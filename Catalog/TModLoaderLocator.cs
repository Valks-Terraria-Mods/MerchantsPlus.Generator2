namespace MerchantsPlus.Generator;

public static class TModLoaderLocator
{
    public static string? ResolveTModLoaderDllPath()
    {
        var settings = CatalogStorage.LoadSettingsConfig();
        var configured = (settings.TModLoaderDllPath ?? string.Empty).Trim();

        if (!string.IsNullOrWhiteSpace(configured) && File.Exists(configured))
            return configured;

        var detected = FindTModLoaderDllPath();
        if (!string.IsNullOrWhiteSpace(detected))
        {
            if (!string.Equals(configured, detected, StringComparison.OrdinalIgnoreCase))
                CatalogStorage.SaveSettingsConfig(new SettingsConfig { TModLoaderDllPath = detected });

            return detected;
        }

        return null;
    }

    public static string? FindTModLoaderDllPath()
    {
        foreach (var path in GetCandidatePaths())
        {
            if (File.Exists(path))
                return path;
        }

        return null;
    }

    public static IReadOnlyList<string> GetCandidatePaths()
    {
        var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var candidates = new List<string>();

        if (OperatingSystem.IsWindows())
        {
            AddSteamCandidate(candidates, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));
            AddSteamCandidate(candidates, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles));
        }
        else if (OperatingSystem.IsMacOS())
        {
            var steamRoot = Path.Combine(home, "Library", "Application Support", "Steam", "steamapps", "common", "tModLoader");
            candidates.Add(Path.Combine(steamRoot, "tModLoader.dll"));
            candidates.Add(Path.Combine(steamRoot, "tModLoader.app", "Contents", "MacOS", "tModLoader.dll"));
            candidates.Add(Path.Combine(steamRoot, "tModLoader.app", "Contents", "Resources", "tModLoader.dll"));
        }
        else
        {
            candidates.Add(Path.Combine(home, ".steam", "steam", "steamapps", "common", "tModLoader", "tModLoader.dll"));
            candidates.Add(Path.Combine(home, ".steam", "root", "steamapps", "common", "tModLoader", "tModLoader.dll"));
            candidates.Add(Path.Combine(home, ".local", "share", "Steam", "steamapps", "common", "tModLoader", "tModLoader.dll"));
            candidates.Add(Path.Combine(home, ".var", "app", "com.valvesoftware.Steam", "data", "Steam", "steamapps", "common", "tModLoader", "tModLoader.dll"));
        }

        return candidates
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }

    private static void AddSteamCandidate(ICollection<string> candidates, string root)
    {
        if (string.IsNullOrWhiteSpace(root))
            return;

        candidates.Add(Path.Combine(root, "Steam", "steamapps", "common", "tModLoader", "tModLoader.dll"));
    }
}
