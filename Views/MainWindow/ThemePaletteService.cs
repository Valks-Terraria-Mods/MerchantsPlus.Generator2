using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace MerchantsPlus.Generator;

public static class ThemePaletteService
{
    public static void Apply(Window window, string appTheme)
    {
        if (Application.Current is not { } app)
            return;

        var normalizedTheme = SettingsDefaults.NormalizeTheme(appTheme);
        app.RequestedThemeVariant = normalizedTheme switch
        {
            var value when string.Equals(value, SettingsDefaults.ThemeLight, StringComparison.OrdinalIgnoreCase) => ThemeVariant.Light,
            var value when string.Equals(value, SettingsDefaults.ThemeSystem, StringComparison.OrdinalIgnoreCase) => ThemeVariant.Default,
            _ => ThemeVariant.Dark
        };

        var useLightPalette = normalizedTheme switch
        {
            var value when string.Equals(value, SettingsDefaults.ThemeLight, StringComparison.OrdinalIgnoreCase) => true,
            var value when string.Equals(value, SettingsDefaults.ThemeDark, StringComparison.OrdinalIgnoreCase) => false,
            _ => app.ActualThemeVariant == ThemeVariant.Light
        };

        if (useLightPalette)
            ApplyLightPalette(window);
        else
            ApplyDarkPalette(window);
    }

    private static void ApplyDarkPalette(Window window)
    {
        SetBrush(window, "AppBackgroundBrush", "#131517");
        SetBrush(window, "PanelBackgroundBrush", "#131517");
        SetBrush(window, "PanelBorderBrush", "#3C434D");
        SetBrush(window, "TextPrimaryBrush", "#CDD2DA");
        SetBrush(window, "TextMutedBrush", "#A9B1BC");
        SetBrush(window, "InputBackgroundBrush", "#23272D");
        SetBrush(window, "InputBorderBrush", "#3B414A");
        SetBrush(window, "InputBorderFocusedBrush", "#5B6572");
        SetBrush(window, "ListBackgroundBrush", "#1A1D22");
        SetBrush(window, "TextControlBackground", "#23272D");
        SetBrush(window, "TextControlBackgroundPointerOver", "#23272D");
        SetBrush(window, "TextControlBackgroundFocused", "#23272D");
        SetBrush(window, "TextControlBorderBrushPointerOver", "#3B414A");
        SetBrush(window, "TextControlBorderBrushFocused", "#5B6572");
    }

    private static void ApplyLightPalette(Window window)
    {
        SetBrush(window, "AppBackgroundBrush", "#EEF2F7");
        SetBrush(window, "PanelBackgroundBrush", "#FFFFFF");
        SetBrush(window, "PanelBorderBrush", "#B7C1CE");
        SetBrush(window, "TextPrimaryBrush", "#25303B");
        SetBrush(window, "TextMutedBrush", "#5E6E82");
        SetBrush(window, "InputBackgroundBrush", "#FFFFFF");
        SetBrush(window, "InputBorderBrush", "#AEB9C7");
        SetBrush(window, "InputBorderFocusedBrush", "#70839A");
        SetBrush(window, "ListBackgroundBrush", "#F7FAFF");
        SetBrush(window, "TextControlBackground", "#FFFFFF");
        SetBrush(window, "TextControlBackgroundPointerOver", "#FFFFFF");
        SetBrush(window, "TextControlBackgroundFocused", "#FFFFFF");
        SetBrush(window, "TextControlBorderBrushPointerOver", "#AEB9C7");
        SetBrush(window, "TextControlBorderBrushFocused", "#70839A");
    }

    private static void SetBrush(Window window, string key, string color)
    {
        window.Resources[key] = new SolidColorBrush(Color.Parse(color));
    }
}
