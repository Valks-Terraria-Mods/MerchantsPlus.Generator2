using System.Reflection;

namespace MerchantsPlus.Generator;

internal static class ItemPriceReflectionEnvironment
{
    internal static Dictionary<string, string> BuildDllLookup(string tmodloaderDllPath)
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

    internal static void WireAssemblyResolver(Dictionary<string, string> dllLookup)
    {
        AppDomain.CurrentDomain.AssemblyResolve += (_, eventArgs) =>
        {
            var assemblyName = new AssemblyName(eventArgs.Name).Name + ".dll";
            return dllLookup.TryGetValue(assemblyName, out var candidate)
                ? Assembly.LoadFrom(candidate)
                : null;
        };
    }

    internal static string? GetTModLoaderDllPath()
    {
        return TModLoaderLocator.ResolveTModLoaderDllPath();
    }

    internal static void InitializeProgramPaths(Assembly assembly)
    {
        var programType = assembly.GetType("Terraria.Program");
        if (programType is null)
            return;

        var saveRoot = Path.Combine(Path.GetTempPath(), "merchantsplus-price-cache");
        Directory.CreateDirectory(saveRoot);

        SetFieldIfWritable(programType, "SavePath", saveRoot);
        SetFieldIfWritable(programType, "TerrariaSaveFolderPath", saveRoot);
        SetFieldIfWritable(programType, "LaunchParameters", new Dictionary<string, string>());
    }

    private static void SetFieldIfWritable(Type type, string fieldName, object value)
    {
        var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        if (field is null || field.IsLiteral || field.IsInitOnly)
            return;

        field.SetValue(null, value);
    }
}
