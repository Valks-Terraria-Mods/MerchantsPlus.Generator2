using Avalonia.Controls;

namespace MerchantsPlus.Generator;

public static class KeywordRuleRowResolver
{
    public static bool TryResolve(object? sender, out KeywordRuleEditorRow row)
    {
        row = null!;
        if (sender is not Control { DataContext: KeywordRuleEditorRow candidate })
            return false;

        row = candidate;
        return true;
    }
}