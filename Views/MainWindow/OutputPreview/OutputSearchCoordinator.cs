namespace MerchantsPlus.Generator;

public sealed class OutputSearchCoordinator{
    private readonly List<OutputSearchMatch> _matches = [];
    private int _currentIndex = -1;

    public IReadOnlyList<OutputSearchMatch> Matches => _matches;
    public int CurrentIndex => _currentIndex;

    public void Rebuild(string jsonText, string searchText)
    {
        _matches.Clear();
        var needle = searchText.Trim();
        if (string.IsNullOrWhiteSpace(needle) || string.IsNullOrEmpty(jsonText))
        {
            _currentIndex = -1;
            return;
        }

        var index = 0;
        while (index < jsonText.Length)
        {
            index = jsonText.IndexOf(needle, index, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                break;

            _matches.Add(new OutputSearchMatch { Start = index, Length = needle.Length });
            index += needle.Length;
        }

        if (_matches.Count == 0)
        {
            _currentIndex = -1;
            return;
        }

        if (_currentIndex < 0 || _currentIndex >= _matches.Count)
            _currentIndex = 0;
    }

    public bool Navigate(int delta)
    {
        if (_matches.Count == 0)
            return false;

        var next = Math.Clamp(_currentIndex + delta, 0, _matches.Count - 1);
        if (next == _currentIndex)
            return false;

        _currentIndex = next;
        return true;
    }

    public void Apply(CatalogViewModel vm)
    {
        vm.OutputSearchMatchCount = _matches.Count;
        vm.OutputSearchMatchIndex = _currentIndex >= 0 ? _currentIndex + 1 : 0;
        vm.CanOutputSearchPrev = _matches.Count > 0 && _currentIndex > 0;
        vm.CanOutputSearchNext = _matches.Count > 0 && _currentIndex < _matches.Count - 1;
    }

    public int GetCurrentMatchLineIndex(string jsonText)
    {
        if (_currentIndex < 0 || _currentIndex >= _matches.Count)
            return -1;

        var lineIndex = 0;
        for (var i = 0; i < _matches[_currentIndex].Start && i < jsonText.Length; i++)
        {
            if (jsonText[i] == '\n')
                lineIndex++;
        }

        return lineIndex;
    }
}
