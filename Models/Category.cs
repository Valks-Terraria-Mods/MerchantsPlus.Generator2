using System.Text;

namespace MerchantsPlus.Generator;

public class Category{
    public required string Name { get; set; }
    public string[] Keywords { get; set; } = [];
    public string[] BlacklistedKeywords { get; set; } = [];
    public KeywordRule[] KeywordRules { get; set; } = [];
    public string[] FirstOrder { get; set; } = [];
    public string[] LastOrder { get; set; } = [];
    public required List<Shop> Shops { get; set; }

    public override string ToString()
    {
        StringBuilder sb = new();
        sb.AppendLine(Name);
        foreach (var shop in Shops)
        {
            sb.AppendLine(shop.ToString());
        }

        return sb.ToString();
    }
}
