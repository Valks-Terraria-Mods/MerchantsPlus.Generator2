using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace MerchantsPlus.Generator;

public sealed class OutputPreviewController{
    private const int OutputJsonHighlightThreshold = 50_000;

    private readonly Func<CatalogViewModel?> _viewModelAccessor;
    private readonly Func<SelectableTextBlock?> _previewAccessor;
    private readonly Func<ScrollViewer?> _scrollViewerAccessor;
    private readonly JsonSegmentBuilder _segmentBuilder;
    private readonly OutputPreviewCache _cache;
    private readonly OutputSearchCoordinator _searchCoordinator;
    private readonly OutputInlineRenderer _inlineRenderer;

    private CancellationTokenSource? _previewCts;
    private string _jsonText = string.Empty;
    private int _previewVersion;

    public OutputPreviewController(
        Func<CatalogViewModel?> viewModelAccessor,
        Func<SelectableTextBlock?> previewAccessor,
        Func<ScrollViewer?> scrollViewerAccessor,
        JsonSegmentBuilder segmentBuilder,
        OutputPreviewCache cache,
        OutputSearchCoordinator searchCoordinator,
        OutputInlineRenderer inlineRenderer)
    {
        _viewModelAccessor = viewModelAccessor;
        _previewAccessor = previewAccessor;
        _scrollViewerAccessor = scrollViewerAccessor;
        _segmentBuilder = segmentBuilder;
        _cache = cache;
        _searchCoordinator = searchCoordinator;
        _inlineRenderer = inlineRenderer;
    }

    public void Refresh()
    {
        var preview = _previewAccessor();
        if (preview is null)
            return;
        if (preview.Inlines is null)
            return;

        _previewVersion++;
        _previewCts?.Cancel();
        _previewCts = new CancellationTokenSource();

        var version = _previewVersion;
        var cancellationToken = _previewCts.Token;

        preview.Inlines.Clear();
        preview.Text = string.Empty;

        var vm = _viewModelAccessor();
        if (vm is null)
            return;

        _jsonText = vm.OutputFileContents ?? string.Empty;
        _searchCoordinator.Rebuild(_jsonText, vm.OutputSearchText ?? string.Empty);
        _searchCoordinator.Apply(vm);

        preview.Text = _jsonText;

        var fileName = vm.SelectedOutputFile ?? string.Empty;
        if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrEmpty(_jsonText) || _jsonText.Length > OutputJsonHighlightThreshold)
        {
            ScrollToCurrentMatch();
            return;
        }

        var lastWriteTimeUtc = GetOutputFileLastWriteTimeUtc(fileName);
        if (_cache.TryGet(fileName, lastWriteTimeUtc, _jsonText.Length, out var segments))
        {
            ApplyInlines(preview, _jsonText, segments, version);
            return;
        }

        var jsonSnapshot = _jsonText;
        _ = Task.Run(() => _segmentBuilder.Build(jsonSnapshot, cancellationToken), cancellationToken)
            .ContinueWith(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                    return;

                if (cancellationToken.IsCancellationRequested || version != _previewVersion)
                    return;

                var builtSegments = task.Result;
                _cache.Store(fileName, lastWriteTimeUtc, jsonSnapshot.Length, builtSegments);

                Dispatcher.UIThread.Post(() =>
                {
                    if (version != _previewVersion)
                        return;

                    var freshPreview = _previewAccessor();
                    if (freshPreview is null)
                        return;

                    ApplyInlines(freshPreview, jsonSnapshot, builtSegments, version);
                });
            }, TaskScheduler.Default);
    }

    public void Navigate(int delta)
    {
        if (!_searchCoordinator.Navigate(delta))
            return;

        var vm = _viewModelAccessor();
        if (vm is not null)
            _searchCoordinator.Apply(vm);

        Refresh();
    }

    public void Dispose()
    {
        _previewCts?.Cancel();
        _previewCts?.Dispose();
    }

    private void ApplyInlines(SelectableTextBlock preview, string json, IReadOnlyList<JsonSegment> segments, int version)
    {
        if (version != _previewVersion)
            return;

        _inlineRenderer.Render(preview, json, segments, _searchCoordinator.Matches, _searchCoordinator.CurrentIndex);
        ScrollToCurrentMatch();
    }

    private void ScrollToCurrentMatch()
    {
        var lineIndex = _searchCoordinator.GetCurrentMatchLineIndex(_jsonText);
        if (lineIndex < 0)
            return;

        var scrollViewer = _scrollViewerAccessor();
        var preview = _previewAccessor();
        if (scrollViewer is null || preview is null)
            return;

        var lineHeight = preview.LineHeight > 0 ? preview.LineHeight : preview.FontSize * 1.4;
        var offsetY = Math.Max(0, lineIndex * lineHeight);
        scrollViewer.Offset = new Vector(scrollViewer.Offset.X, offsetY);
    }

    private static DateTime GetOutputFileLastWriteTimeUtc(string fileName)
    {
        var path = CatalogStorage.GetOutputFilePath(fileName);
        return string.IsNullOrWhiteSpace(path) || !File.Exists(path)
            ? DateTime.MinValue
            : File.GetLastWriteTimeUtc(path);
    }
}
