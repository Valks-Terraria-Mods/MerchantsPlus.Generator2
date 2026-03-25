using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;

namespace MerchantsPlus.Generator;

public sealed class MainWindowLifecycleCoordinator : IDisposable
{
    private readonly Window _window;
    private readonly CatalogViewModelAccessor _viewModelAccessor;
    private readonly OutputPreviewController _outputPreviewController;
    private CatalogViewModel? _trackedViewModel;

    public MainWindowLifecycleCoordinator(
        Window window,
        CatalogViewModelAccessor viewModelAccessor,
        OutputPreviewController outputPreviewController)
    {
        _window = window;
        _viewModelAccessor = viewModelAccessor;
        _outputPreviewController = outputPreviewController;
    }

    public void HandleDataContextChanged()
    {
        if (_trackedViewModel is not null)
            _trackedViewModel.PropertyChanged -= OnTrackedViewModelPropertyChanged;

        _trackedViewModel = _viewModelAccessor.Current;

        if (_trackedViewModel is not null)
            _trackedViewModel.PropertyChanged += OnTrackedViewModelPropertyChanged;

        ApplyThemeFromViewModel();
        _outputPreviewController.Refresh();
    }

    public void Dispose()
    {
        if (_trackedViewModel is not null)
            _trackedViewModel.PropertyChanged -= OnTrackedViewModelPropertyChanged;
    }

    private void OnTrackedViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(CatalogViewModel.OutputFileContents)
            or nameof(CatalogViewModel.OutputSearchText)
            or nameof(CatalogViewModel.SelectedOutputFile))
            _outputPreviewController.Refresh();

        if (e.PropertyName is nameof(CatalogViewModel.AppTheme))
            ApplyThemeFromViewModel();
    }

    private void ApplyThemeFromViewModel()
    {
        if (Application.Current is null || _viewModelAccessor.Current is not { } viewModel)
            return;

        ThemePaletteService.Apply(_window, viewModel.AppTheme);
    }
}