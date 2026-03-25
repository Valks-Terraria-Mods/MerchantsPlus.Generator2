namespace MerchantsPlus.Generator;

public sealed class OutputPreviewCache{
    private readonly int _limit;
    private readonly Dictionary<string, CacheEntry> _cache = new(StringComparer.OrdinalIgnoreCase);

    private sealed class CacheEntry
    {
        public required DateTime LastWriteTimeUtc { get; init; }
        public required int TextLength { get; init; }
        public required List<JsonSegment> Segments { get; init; }
        public DateTime LastAccessUtc { get; set; }
    }

    public OutputPreviewCache(int limit)
    {
        _limit = Math.Max(1, limit);
    }

    public bool TryGet(string fileName, DateTime lastWriteTimeUtc, int textLength, out List<JsonSegment> segments)
    {
        if (_cache.TryGetValue(fileName, out var cache)
            && cache.LastWriteTimeUtc == lastWriteTimeUtc
            && cache.TextLength == textLength)
        {
            cache.LastAccessUtc = DateTime.UtcNow;
            segments = cache.Segments;
            return true;
        }

        segments = [];
        return false;
    }

    public void Store(string fileName, DateTime lastWriteTimeUtc, int textLength, List<JsonSegment> segments)
    {
        _cache[fileName] = new CacheEntry
        {
            LastWriteTimeUtc = lastWriteTimeUtc,
            TextLength = textLength,
            Segments = segments,
            LastAccessUtc = DateTime.UtcNow
        };

        if (_cache.Count <= _limit)
            return;

        foreach (var entry in _cache
                     .OrderBy(pair => pair.Value.LastAccessUtc)
                     .Take(_cache.Count - _limit)
                     .ToList())
        {
            _cache.Remove(entry.Key);
        }
    }
}
