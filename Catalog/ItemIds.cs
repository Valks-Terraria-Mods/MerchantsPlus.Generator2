using System.Reflection;

namespace MerchantsPlus.Generator;

public static class ItemIds
{
    public static IEnumerable<string> GetAll()
    {
        var tmodloaderDllPath = GetTModLoaderDllPath()
            ?? throw new InvalidOperationException("Unable to find tModLoader.dll");

        var dllLookup = BuildDllLookup(tmodloaderDllPath);
        WireAssemblyResolver(dllLookup);

        var itemIdType = LoadItemIdType(tmodloaderDllPath);
        var fields = GetItemIdFields(itemIdType);

        return fields
            .Where(IsItemField)
            .Select(f => f.Name);
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

    private static Type LoadItemIdType(string tmodloaderDllPath)
    {
        var assembly = Assembly.LoadFrom(tmodloaderDllPath);
        return assembly.GetType("Terraria.ID.ItemID")
            ?? throw new InvalidOperationException("Unable to find Terraria.ID.ItemID type");
    }

    private static FieldInfo[] GetItemIdFields(Type itemIdType)
    {
        return itemIdType.GetFields(BindingFlags.Public | BindingFlags.Static);
    }

    private static bool IsItemField(FieldInfo field)
    {
        if (!field.IsLiteral || field.IsInitOnly)
            return false;

        return field.FieldType == typeof(int) || field.FieldType == typeof(short);
    }

    private static string? GetTModLoaderDllPath()
    {
        return TModLoaderLocator.ResolveTModLoaderDllPath();
    }
}
