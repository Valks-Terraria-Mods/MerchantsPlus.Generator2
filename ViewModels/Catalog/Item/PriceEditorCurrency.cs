namespace MerchantsPlus.Generator;

internal static class PriceEditorCurrency
{
    internal static string NormalizeField(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "0";

        var digits = new string(value.Where(char.IsDigit).ToArray());
        if (string.IsNullOrWhiteSpace(digits))
            return "0";

        var trimmed = digits.TrimStart('0');
        return trimmed.Length > 0 ? trimmed : "0";
    }

    internal static int ParseNormalizedCopper(string platinumText, string goldText, string silverText, string copperText, int maxCopper)
    {
        var platinum = ParsePositiveInt(platinumText);
        var gold = ParsePositiveInt(goldText);
        var silver = ParsePositiveInt(silverText);
        var copper = ParsePositiveInt(copperText);

        silver += copper / 100;
        copper %= 100;
        gold += silver / 100;
        silver %= 100;
        platinum += gold / 100;
        gold %= 100;

        return Math.Min(ItemPrices.ToCopper(platinum, gold, silver, copper), maxCopper);
    }

    private static int ParsePositiveInt(string value)
    {
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : 0;
    }
}