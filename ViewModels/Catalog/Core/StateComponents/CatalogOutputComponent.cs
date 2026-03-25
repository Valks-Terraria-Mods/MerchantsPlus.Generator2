namespace MerchantsPlus.Generator;

public sealed class CatalogOutputComponent{
    private string _selectedOutputFile = string.Empty;
    private string _outputFileContents = string.Empty;
    private string _outputSearchText = string.Empty;
    private int _outputSearchMatchCount;
    private int _outputSearchMatchIndex;
    private bool _canOutputSearchPrev;
    private bool _canOutputSearchNext;

    public string SelectedOutputFile { get => _selectedOutputFile; set => _selectedOutputFile = value; }
    public string OutputFileContents { get => _outputFileContents; set => _outputFileContents = value; }
    public string OutputSearchText { get => _outputSearchText; set => _outputSearchText = value; }
    public int OutputSearchMatchCount { get => _outputSearchMatchCount; set => _outputSearchMatchCount = value; }
    public int OutputSearchMatchIndex { get => _outputSearchMatchIndex; set => _outputSearchMatchIndex = value; }
    public bool CanOutputSearchPrev { get => _canOutputSearchPrev; set => _canOutputSearchPrev = value; }
    public bool CanOutputSearchNext { get => _canOutputSearchNext; set => _canOutputSearchNext = value; }
}
