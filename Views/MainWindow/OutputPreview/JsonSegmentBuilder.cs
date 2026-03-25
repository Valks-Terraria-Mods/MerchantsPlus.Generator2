namespace MerchantsPlus.Generator;

public sealed class JsonSegmentBuilder{
    public List<JsonSegment> Build(string json, CancellationToken cancellationToken)
    {
        var segments = new List<JsonSegment>();
        var index = 0;

        while (index < json.Length)
        {
            if (cancellationToken.IsCancellationRequested)
                return segments;

            var ch = json[index];
            if (ch == '"')
            {
                var start = index;
                index++;
                var escaped = false;
                while (index < json.Length)
                {
                    var current = json[index];
                    if (escaped)
                    {
                        escaped = false;
                        index++;
                        continue;
                    }

                    if (current == '\\')
                    {
                        escaped = true;
                        index++;
                        continue;
                    }

                    index++;
                    if (current == '"')
                        break;
                }

                var length = index - start;
                AddSegment(segments, start, length, IsNextNonWhitespaceColon(json, index) ? JsonTokenStyle.Key : JsonTokenStyle.String);
                continue;
            }

            if (ch == '\r' || ch == '\n')
            {
                var lineBreakLength = ch == '\r' && index + 1 < json.Length && json[index + 1] == '\n' ? 2 : 1;
                segments.Add(new JsonSegment
                {
                    Start = index,
                    Length = lineBreakLength,
                    TokenStyle = JsonTokenStyle.Default,
                    IsLineBreak = true
                });
                index += lineBreakLength;
                continue;
            }

            if (char.IsWhiteSpace(ch))
            {
                var start = index;
                index++;
                while (index < json.Length && char.IsWhiteSpace(json[index]) && json[index] != '\r' && json[index] != '\n')
                    index++;

                AddSegment(segments, start, index - start, JsonTokenStyle.Default);
                continue;
            }

            if (ch == '-' || char.IsDigit(ch))
            {
                var start = index;
                index++;
                while (index < json.Length && (char.IsDigit(json[index]) || json[index] == '.' || json[index] == 'e' || json[index] == 'E' || json[index] == '+' || json[index] == '-'))
                    index++;

                AddSegment(segments, start, index - start, JsonTokenStyle.Number);
                continue;
            }

            if (char.IsLetter(ch))
            {
                var start = index;
                index++;
                while (index < json.Length && char.IsLetter(json[index]))
                    index++;

                var length = index - start;
                var tokenStyle = IsBooleanLiteral(json, start, length) ? JsonTokenStyle.Boolean : JsonTokenStyle.Default;
                AddSegment(segments, start, length, tokenStyle);
                continue;
            }

            AddSegment(segments, index, 1, JsonTokenStyle.Punctuation);
            index++;
        }

        return segments;
    }

    private static bool IsBooleanLiteral(string json, int start, int length)
    {
        return length switch
        {
            4 => EqualsIgnoreCase(json, start, "true") || EqualsIgnoreCase(json, start, "null"),
            5 => EqualsIgnoreCase(json, start, "false"),
            _ => false
        };
    }

    private static bool EqualsIgnoreCase(string text, int start, string keyword)
    {
        if (start + keyword.Length > text.Length)
            return false;

        for (var i = 0; i < keyword.Length; i++)
        {
            if (char.ToLowerInvariant(text[start + i]) != keyword[i])
                return false;
        }

        return true;
    }

    private static bool IsNextNonWhitespaceColon(string json, int index)
    {
        while (index < json.Length && char.IsWhiteSpace(json[index]))
            index++;

        return index < json.Length && json[index] == ':';
    }

    private static void AddSegment(List<JsonSegment> segments, int start, int length, JsonTokenStyle tokenStyle)
    {
        if (length <= 0)
            return;

        if (segments.Count > 0)
        {
            var last = segments[^1];
            if (!last.IsLineBreak && last.TokenStyle == tokenStyle && last.Start + last.Length == start)
            {
                last.Length += length;
                return;
            }
        }

        segments.Add(new JsonSegment
        {
            Start = start,
            Length = length,
            TokenStyle = tokenStyle,
            IsLineBreak = false
        });
    }
}
