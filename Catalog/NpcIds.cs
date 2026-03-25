using System.Reflection;
using System.Text.RegularExpressions;

namespace MerchantsPlus.Generator;

public static class NpcIds
{

    public static IEnumerable<string> GetTownNpcIds()
    {
        return GetMerchantNpcIds();
    }

    public static IEnumerable<string> GetMerchantNpcIds(string? modSourceRoot = null)
    {
        var tmodloaderDllPath = TModLoaderLocator.ResolveTModLoaderDllPath()
            ?? throw new InvalidOperationException("Unable to find tModLoader.dll");

        var settings = CatalogStorage.LoadSettingsConfig();
        var allowedMerchantNpcIds = GetConfiguredSet(settings.AllowedMerchantNpcIds, SettingsDefaults.AllowedMerchantNpcIds);
        var alwaysIncludeMerchantNpcIds = GetConfiguredSet(settings.AlwaysIncludeMerchantNpcIds, SettingsDefaults.AlwaysIncludeMerchantNpcIds);

        var dllLookup = BuildDllLookup(tmodloaderDllPath);
        WireAssemblyResolver(dllLookup);

        var assembly = Assembly.LoadFrom(tmodloaderDllPath);
        var npcIdType = assembly.GetType("Terraria.ID.NPCID")
            ?? throw new InvalidOperationException("Unable to find Terraria.ID.NPCID type");

        var setsType = assembly.GetType("Terraria.ID.NPCID+Sets")
            ?? throw new InvalidOperationException("Unable to find Terraria.ID.NPCID+Sets type");

        var townNpcField = setsType.GetField("TownNPC", BindingFlags.Public | BindingFlags.Static)
            ?? setsType.GetField("ShimmerTownTransform", BindingFlags.Public | BindingFlags.Static)
            ?? setsType.GetField("ActsLikeTownNPC", BindingFlags.Public | BindingFlags.Static)
            ?? throw new InvalidOperationException("Unable to find a town NPC bool[] field (TownNPC/ShimmerTownTransform/ActsLikeTownNPC)");

        if (townNpcField.GetValue(null) is not bool[] townNpcFlags)
            throw new InvalidOperationException("Terraria.ID.NPCID.Sets.TownNPC was not a bool[]");

        var npcConstantNames = npcIdType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(IsNpcField)
            .Select(field => field.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var results = npcIdType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(IsNpcField)
            .Where(field => IsTownNpc(field, townNpcFlags))
            .Select(field => field.Name)
            .Where(name => allowedMerchantNpcIds.Contains(name))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        foreach (var referencedNpc in GetReferencedNpcIds(modSourceRoot))
        {
            if (npcConstantNames.Contains(referencedNpc) && allowedMerchantNpcIds.Contains(referencedNpc))
                results.Add(referencedNpc);
        }

        foreach (var alwaysInclude in alwaysIncludeMerchantNpcIds)
        {
            if (npcConstantNames.Contains(alwaysInclude))
                results.Add(alwaysInclude);
        }

        foreach (var allowed in allowedMerchantNpcIds)
        {
            if (npcConstantNames.Contains(allowed))
                results.Add(allowed);
        }

        return results.OrderBy(name => name, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsTownNpc(FieldInfo field, bool[] townNpcFlags)
    {
        var rawValue = field.GetRawConstantValue();
        if (rawValue is null)
            return false;

        var id = Convert.ToInt32(rawValue);
        return id >= 0 && id < townNpcFlags.Length && townNpcFlags[id];
    }

    private static HashSet<string> GetConfiguredSet(string[]? configured, IReadOnlyList<string> fallback)
    {
        if (configured is { Length: > 0 })
            return new HashSet<string>(configured, StringComparer.OrdinalIgnoreCase);

        return new HashSet<string>(fallback, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsNpcField(FieldInfo field)
    {
        if (!field.IsLiteral || field.IsInitOnly)
            return false;

        return field.FieldType == typeof(int) || field.FieldType == typeof(short);
    }

    private static IEnumerable<string> GetReferencedNpcIds(string? modSourceRoot)
    {
        var root = modSourceRoot;
        if (string.IsNullOrWhiteSpace(root))
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            root = Path.Combine(home, ".local/share/Terraria/tModLoader/ModSources/MerchantsPlus/MerchantsPlus");
        }

        if (string.IsNullOrWhiteSpace(root) || !Directory.Exists(root))
            return [];

        var regex = new Regex(@"\bNPCID\.([A-Za-z_][A-Za-z0-9_]*)\b", RegexOptions.Compiled);
        var results = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var file in Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories))
        {
            var text = File.ReadAllText(file);
            var matches = regex.Matches(text);
            foreach (Match match in matches)
            {
                if (match.Groups.Count < 2)
                    continue;

                var name = match.Groups[1].Value;
                if (!string.IsNullOrWhiteSpace(name))
                    results.Add(name);
            }
        }

        return results;
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

}
