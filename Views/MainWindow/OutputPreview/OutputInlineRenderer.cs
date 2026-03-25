using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace MerchantsPlus.Generator;

public sealed class OutputInlineRenderer{
    private readonly Dictionary<JsonTokenStyle, IBrush> _foregroundBrushes;
    private readonly IBrush _searchHighlightBrush;
    private readonly IBrush _searchCurrentBrush;

    public OutputInlineRenderer()
    {
        _foregroundBrushes = new Dictionary<JsonTokenStyle, IBrush>
        {
            [JsonTokenStyle.Default] = new SolidColorBrush(Color.Parse("#E2E8F0")),
            [JsonTokenStyle.Key] = new SolidColorBrush(Color.Parse("#93C5FD")),
            [JsonTokenStyle.String] = new SolidColorBrush(Color.Parse("#FCA5A5")),
            [JsonTokenStyle.Number] = new SolidColorBrush(Color.Parse("#FDE047")),
            [JsonTokenStyle.Boolean] = new SolidColorBrush(Color.Parse("#C4B5FD")),
            [JsonTokenStyle.Punctuation] = new SolidColorBrush(Color.Parse("#94A3B8"))
        };

        _searchHighlightBrush = new SolidColorBrush(Color.Parse("#3B3A2A"));
        _searchCurrentBrush = new SolidColorBrush(Color.Parse("#7C2D12"));
    }

    public void Render(
        SelectableTextBlock preview,
        string json,
        IReadOnlyList<JsonSegment> segments,
        IReadOnlyList<OutputSearchMatch> matches,
        int currentMatchIndex)
    {
        if (preview.Inlines is null)
            return;

        preview.Inlines.Clear();
        preview.Text = string.Empty;

        var matchPointer = 0;
        foreach (var segment in segments)
        {
            if (segment.IsLineBreak)
            {
                preview.Inlines.Add(new LineBreak());
                continue;
            }

            if (segment.Length <= 0)
                continue;

            var token = json.Substring(segment.Start, segment.Length);
            AddRunsForRange(preview.Inlines, token, segment.Start, GetForegroundBrush(segment.TokenStyle), matches, currentMatchIndex, _searchCurrentBrush, _searchHighlightBrush, ref matchPointer);
        }
    }

    private IBrush GetForegroundBrush(JsonTokenStyle tokenStyle)
    {
        if (_foregroundBrushes.TryGetValue(tokenStyle, out var brush))
            return brush;

        return _foregroundBrushes[JsonTokenStyle.Default];
    }

    private static void AddRunsForRange(
        ICollection<Inline> inlines,
        string text,
        int globalStart,
        IBrush foreground,
        IReadOnlyList<OutputSearchMatch> matches,
        int currentMatchIndex,
        IBrush searchCurrentBrush,
        IBrush searchHighlightBrush,
        ref int matchPointer)
    {
        if (string.IsNullOrEmpty(text))
            return;

        if (matches.Count == 0)
        {
            inlines.Add(new Run(text) { Foreground = foreground });
            return;
        }

        var globalEnd = globalStart + text.Length;
        while (matchPointer < matches.Count && matches[matchPointer].Start + matches[matchPointer].Length <= globalStart)
            matchPointer++;

        var currentGlobal = globalStart;
        var pointer = matchPointer;
        while (pointer < matches.Count)
        {
            var match = matches[pointer];
            var matchStart = match.Start;
            var matchEnd = match.Start + match.Length;
            if (matchStart >= globalEnd)
                break;

            var segmentStart = Math.Max(matchStart, globalStart);
            var segmentEnd = Math.Min(matchEnd, globalEnd);
            if (segmentStart > currentGlobal)
                inlines.Add(new Run(text.Substring(currentGlobal - globalStart, segmentStart - currentGlobal)) { Foreground = foreground });

            if (segmentEnd > segmentStart)
            {
                inlines.Add(new Run(text.Substring(segmentStart - globalStart, segmentEnd - segmentStart))
                {
                    Foreground = foreground,
                    Background = pointer == currentMatchIndex ? searchCurrentBrush : searchHighlightBrush
                });
            }

            currentGlobal = segmentEnd;
            if (matchEnd <= globalEnd)
                pointer++;
            else
                break;
        }

        if (currentGlobal < globalEnd)
            inlines.Add(new Run(text.Substring(currentGlobal - globalStart, globalEnd - currentGlobal)) { Foreground = foreground });

        matchPointer = pointer;
    }
}
