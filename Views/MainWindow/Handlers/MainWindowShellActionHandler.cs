namespace MerchantsPlus.Generator;

public sealed class MainWindowShellActionHandler
{
    private readonly CatalogViewModelAccessor _viewModelAccessor;
    private readonly MainWindowOverlayController _overlayController;
    private readonly OutputPreviewController _outputPreviewController;
    private readonly Action _closeWindow;

    public MainWindowShellActionHandler(
        CatalogViewModelAccessor viewModelAccessor,
        MainWindowOverlayController overlayController,
        OutputPreviewController outputPreviewController,
        Action closeWindow)
    {
        _viewModelAccessor = viewModelAccessor;
        _overlayController = overlayController;
        _outputPreviewController = outputPreviewController;
        _closeWindow = closeWindow;
    }

    public void ForceRefresh() => _viewModelAccessor.Execute(vm => vm.ForceRefreshAllJsonOutputs());

    public void Exit() => _closeWindow();

    public void OpenOutputFolder() => _viewModelAccessor.Execute(vm => vm.OpenOutputFolder());

    public void GenerateCatalogScript() => _viewModelAccessor.Execute(vm => vm.GenerateCatalogDataScript());

    public void OutputSearchPrevious() => _outputPreviewController.Navigate(-1);

    public void OutputSearchNext() => _outputPreviewController.Navigate(1);

    public void OpenSettings()
    {
        _viewModelAccessor.Execute(vm => vm.LoadSettings());
        _overlayController.Open("SettingsOverlay");
    }

    public void CloseSettings() => _overlayController.Close("SettingsOverlay");

    public void AutoDetectTModLoader() => _viewModelAccessor.Execute(vm => vm.AutoDetectTModLoaderDllPath());

    public void SaveSettings() => _viewModelAccessor.Execute(vm => vm.SaveSettings());

    public void OpenGlobalFlags()
    {
        _viewModelAccessor.Execute(vm => vm.LoadGlobalFlagsSettings());
        _overlayController.Open("GlobalFlagsOverlay");
    }

    public void CloseGlobalFlags() => _overlayController.Close("GlobalFlagsOverlay");

    public void CloseSettingsFromBackdrop(Avalonia.Input.PointerPressedEventArgs e) => _overlayController.CloseFromBackdrop("SettingsOverlay", e);

    public void CloseGlobalFlagsFromBackdrop(Avalonia.Input.PointerPressedEventArgs e) => _overlayController.CloseFromBackdrop("GlobalFlagsOverlay", e);
}