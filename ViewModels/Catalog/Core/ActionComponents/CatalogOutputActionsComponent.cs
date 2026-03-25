using System.Diagnostics;

namespace MerchantsPlus.Generator;

public sealed class CatalogOutputActionsComponent{
    private sealed class CachedOutputFile
    {
        public required string Contents { get; init; }
        public required DateTime LastWriteTimeUtc { get; init; }
    }

    private readonly Dictionary<string, CachedOutputFile> _outputFileCache = new(StringComparer.OrdinalIgnoreCase);
    private readonly GeneratedCatalogScriptGenerator _generatedCatalogScriptGenerator = new();

    public IReadOnlyList<string> ListOutputFiles()
    {
        return CatalogStorage.ListOutputFiles().ToArray();
    }

    public string LoadOutputFileContents(string selectedOutputFile)
    {
        if (string.IsNullOrWhiteSpace(selectedOutputFile))
            return string.Empty;

        var path = CatalogStorage.GetOutputFilePath(selectedOutputFile);
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            return string.Empty;

        var lastWriteTimeUtc = File.GetLastWriteTimeUtc(path);
        if (_outputFileCache.TryGetValue(path, out var cached) && cached.LastWriteTimeUtc == lastWriteTimeUtc)
            return cached.Contents;

        var contents = File.ReadAllText(path);
        _outputFileCache[path] = new CachedOutputFile
        {
            Contents = contents,
            LastWriteTimeUtc = lastWriteTimeUtc
        };

        return contents;
    }

    public void OpenOutputFolder()
    {
        var path = CatalogStorage.GetOutputDirectory();
        Process.Start(new ProcessStartInfo
        {
            FileName = path,
            UseShellExecute = true
        });
    }

    public GeneratedCatalogScriptResult GenerateCatalogDataScript(string className, bool overwrite)
    {
        return _generatedCatalogScriptGenerator.Generate(className, overwrite);
    }
}
