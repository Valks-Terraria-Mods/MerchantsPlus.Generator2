using System.Text;
using System.Text.Json;

namespace MerchantsPlus.Generator;

internal sealed class GeneratedCatalogScriptGenerator
{
    private const string GeneratedDirectoryName = "generated";
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly GeneratedCatalogScriptBuilder _scriptBuilder = new();

    public GeneratedCatalogScriptResult Generate(string className, bool overwrite)
    {
        var normalizedClassName = NormalizeClassName(className);
        if (string.IsNullOrWhiteSpace(normalizedClassName))
        {
            return new GeneratedCatalogScriptResult
            {
                Success = false,
                Message = "Generate failed: class name is empty or invalid.",
                OutputPath = string.Empty
            };
        }

        var outputFiles = CatalogStorage.ListOutputFiles();
        if (outputFiles.Count == 0)
        {
            return new GeneratedCatalogScriptResult
            {
                Success = false,
                Message = "Generate failed: no output JSON files found in catalog-data/output.",
                OutputPath = string.Empty
            };
        }

        var merchants = new List<MerchantOutput>();
        var skippedFiles = new List<string>();
        foreach (var fileName in outputFiles)
        {
            if (TryLoadMerchant(fileName, out var merchant))
            {
                merchants.Add(merchant);
                continue;
            }

            skippedFiles.Add(fileName);
        }

        if (merchants.Count == 0)
        {
            return new GeneratedCatalogScriptResult
            {
                Success = false,
                Message = "Generate failed: none of the output files could be parsed as merchant output JSON.",
                OutputPath = string.Empty
            };
        }

        var generatedDirectory = Path.Combine(Environment.CurrentDirectory, "catalog-data", GeneratedDirectoryName);
        Directory.CreateDirectory(generatedDirectory);

        var outputPath = Path.Combine(generatedDirectory, $"{normalizedClassName}.cs");
        if (File.Exists(outputPath) && !overwrite)
        {
            return new GeneratedCatalogScriptResult
            {
                Success = false,
                Message = $"Generate failed: {outputPath} already exists and overwrite is disabled.",
                OutputPath = outputPath
            };
        }

        var outputText = _scriptBuilder.Build(normalizedClassName, merchants);
        File.WriteAllText(outputPath, outputText, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false));

        var status = BuildStatusMessage(normalizedClassName, outputPath, merchants.Count, skippedFiles);
        return new GeneratedCatalogScriptResult
        {
            Success = true,
            Message = status,
            OutputPath = outputPath
        };
    }

    private static bool TryLoadMerchant(string fileName, out MerchantOutput merchant)
    {
        merchant = new MerchantOutput();

        var json = CatalogStorage.LoadOutputFile(fileName);
        if (string.IsNullOrWhiteSpace(json))
            return false;

        try
        {
            var parsed = JsonSerializer.Deserialize<MerchantOutput>(json, JsonOptions);
            if (parsed is null || string.IsNullOrWhiteSpace(parsed.Name))
                return false;

            merchant = parsed;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static string BuildStatusMessage(string className, string outputPath, int merchantCount, IReadOnlyList<string> skippedFiles)
    {
        var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, outputPath);
        var status = $"Generated {className} with {merchantCount} merchants at {relativePath}.";

        if (skippedFiles.Count == 0)
            return status;

        var skipped = string.Join(", ", skippedFiles);
        return $"{status} Skipped files: {skipped}.";
    }

    private static string NormalizeClassName(string className)
    {
        if (string.IsNullOrWhiteSpace(className))
            return string.Empty;

        var source = className.Trim();
        var builder = new StringBuilder(source.Length + 4);

        if (!IsIdentifierStartCharacter(source[0]))
            builder.Append('G');

        foreach (var ch in source)
        {
            if (char.IsLetterOrDigit(ch) || ch == '_')
                builder.Append(ch);
            else
                builder.Append('_');
        }

        return builder.ToString();
    }

    private static bool IsIdentifierStartCharacter(char value)
    {
        return char.IsLetter(value) || value == '_';
    }
}
