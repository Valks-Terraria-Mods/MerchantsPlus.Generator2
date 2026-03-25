namespace MerchantsPlus.Generator;

public sealed class JsonSegment{
    public int Start { get; set; }
    public int Length { get; set; }
    public required JsonTokenStyle TokenStyle { get; init; }
    public bool IsLineBreak { get; init; }
}
