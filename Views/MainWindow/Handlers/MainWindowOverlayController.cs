using Avalonia.Controls;
using Avalonia.Input;

namespace MerchantsPlus.Generator;

public sealed class MainWindowOverlayController
{
    private readonly Window _window;

    public MainWindowOverlayController(Window window)
    {
        _window = window;
    }

    public void Open(string overlayName) => SetVisibility(overlayName, isVisible: true);

    public void Close(string overlayName) => SetVisibility(overlayName, isVisible: false);

    public void CloseFromBackdrop(string overlayName, PointerPressedEventArgs _) => Close(overlayName);

    public static void ConsumePanelPointer(PointerPressedEventArgs e) => e.Handled = true;

    private void SetVisibility(string overlayName, bool isVisible)
    {
        if (_window.FindControl<Control>(overlayName) is { } overlay)
            overlay.IsVisible = isVisible;
    }
}