using System.Reflection;

namespace MerchantsPlus.Generator;

public static class ProgressionFlags
{
    private static IReadOnlyList<string>? _cachedFlags;

    public static IReadOnlyList<string> GetWorldProgressionFlags()
    {
        if (_cachedFlags is not null)
            return _cachedFlags;

        var tmodloaderDllPath = TModLoaderLocator.ResolveTModLoaderDllPath()
            ?? throw new InvalidOperationException("Unable to find tModLoader.dll");

        var dllLookup = BuildDllLookup(tmodloaderDllPath);
        WireAssemblyResolver(dllLookup);

        var assembly = Assembly.LoadFrom(tmodloaderDllPath);
        var npcType = assembly.GetType("Terraria.NPC")
            ?? throw new InvalidOperationException("Unable to find Terraria.NPC type");

        var mainType = assembly.GetType("Terraria.Main")
            ?? throw new InvalidOperationException("Unable to find Terraria.Main type");

        var flags = new List<string>();

        var settings = CatalogStorage.LoadSettingsConfig();
        var downedPrefixes = settings.DownedFlagPrefixes is { Length: > 0 } ? settings.DownedFlagPrefixes : SettingsDefaults.DownedFlagPrefixes;

        flags.AddRange(
            npcType.GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(field => field.FieldType == typeof(bool) && StartsWithAny(field.Name, downedPrefixes))
                .Select(field => $"NPC.{field.Name}"));

        var mainFlags = settings.MainFlags is { Length: > 0 } ? settings.MainFlags : SettingsDefaults.MainFlags;
        foreach (var flag in mainFlags)
        {
            var trimmed = flag?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed))
                continue;

            if (!trimmed.StartsWith("Main.", StringComparison.OrdinalIgnoreCase))
                continue;

            var fieldName = trimmed["Main.".Length..];
            if (string.IsNullOrWhiteSpace(fieldName))
                continue;

            var field = mainType.GetField(fieldName, BindingFlags.Public | BindingFlags.Static);
            if (field is not null && field.FieldType == typeof(bool))
                flags.Add(trimmed);
        }

        _cachedFlags =
        [
            .. flags
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(flag => flag, StringComparer.OrdinalIgnoreCase)
        ];

        return _cachedFlags;
    }

    private static Dictionary<string, string> BuildDllLookup(string tmodloaderDllPath)
    {
        var tmodloaderDir = Path.GetDirectoryName(tmodloaderDllPath)!;
        var dllLookup = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (var path in Directory.EnumerateFiles(tmodloaderDir, "*.dll", SearchOption.AllDirectories))
        {
            var fileName = Path.GetFileName(path);
            if (!dllLookup.ContainsKey(fileName))
                dllLookup[fileName] = path;
        }

        return dllLookup;
    }

    private static void WireAssemblyResolver(Dictionary<string, string> dllLookup)
    {
        AppDomain.CurrentDomain.AssemblyResolve += (_, eventArgs) =>
        {
            var assemblyName = new AssemblyName(eventArgs.Name).Name + ".dll";
            return dllLookup.TryGetValue(assemblyName, out var candidate)
                ? Assembly.LoadFrom(candidate)
                : null;
        };
    }

    private static bool StartsWithAny(string value, IEnumerable<string> prefixes)
    {
        foreach (var prefix in prefixes)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                continue;

            if (value.StartsWith(prefix.Trim(), StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

}
