using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace MerchantsPlus.Generator;

public partial class CountBadgeView : UserControl
{
    private readonly Border _badgeBorder;
    private readonly TextBlock _badgeTextBlock;

    static CountBadgeView()
    {
        BadgeTextProperty.Changed.AddClassHandler<CountBadgeView>((control, _) => control.ApplyText());
        BadgeBackgroundProperty.Changed.AddClassHandler<CountBadgeView>((control, _) => control.ApplyBackground());
        BadgeForegroundProperty.Changed.AddClassHandler<CountBadgeView>((control, _) => control.ApplyForeground());
        BadgeCornerRadiusProperty.Changed.AddClassHandler<CountBadgeView>((control, _) => control.ApplyCornerRadius());
        BadgePaddingProperty.Changed.AddClassHandler<CountBadgeView>((control, _) => control.ApplyPadding());
        BadgeTextFontSizeProperty.Changed.AddClassHandler<CountBadgeView>((control, _) => control.ApplyFontSize());
    }

    public static readonly StyledProperty<string?> BadgeTextProperty =
        AvaloniaProperty.Register<CountBadgeView, string?>(nameof(BadgeText), string.Empty);

    public static readonly StyledProperty<IBrush?> BadgeBackgroundProperty =
        AvaloniaProperty.Register<CountBadgeView, IBrush?>(nameof(BadgeBackground), Brushes.Transparent);

    public static readonly StyledProperty<IBrush?> BadgeForegroundProperty =
        AvaloniaProperty.Register<CountBadgeView, IBrush?>(nameof(BadgeForeground), Brushes.White);

    public static readonly StyledProperty<CornerRadius> BadgeCornerRadiusProperty =
        AvaloniaProperty.Register<CountBadgeView, CornerRadius>(nameof(BadgeCornerRadius), new CornerRadius(8));

    public static readonly StyledProperty<Thickness> BadgePaddingProperty =
        AvaloniaProperty.Register<CountBadgeView, Thickness>(nameof(BadgePadding), new Thickness(8, 2));

    public static readonly StyledProperty<double> BadgeTextFontSizeProperty =
        AvaloniaProperty.Register<CountBadgeView, double>(nameof(BadgeTextFontSize), 12);

    public CountBadgeView()
    {
        InitializeComponent();

        _badgeBorder = this.FindControl<Border>("BadgeBorder")
            ?? throw new InvalidOperationException("BadgeBorder was not found.");
        _badgeTextBlock = this.FindControl<TextBlock>("BadgeTextBlock")
            ?? throw new InvalidOperationException("BadgeTextBlock was not found.");

        ApplyAll();
    }

    public string? BadgeText
    {
        get => GetValue(BadgeTextProperty);
        set => SetValue(BadgeTextProperty, value);
    }

    public IBrush? BadgeBackground
    {
        get => GetValue(BadgeBackgroundProperty);
        set => SetValue(BadgeBackgroundProperty, value);
    }

    public IBrush? BadgeForeground
    {
        get => GetValue(BadgeForegroundProperty);
        set => SetValue(BadgeForegroundProperty, value);
    }

    public CornerRadius BadgeCornerRadius
    {
        get => GetValue(BadgeCornerRadiusProperty);
        set => SetValue(BadgeCornerRadiusProperty, value);
    }

    public Thickness BadgePadding
    {
        get => GetValue(BadgePaddingProperty);
        set => SetValue(BadgePaddingProperty, value);
    }

    public double BadgeTextFontSize
    {
        get => GetValue(BadgeTextFontSizeProperty);
        set => SetValue(BadgeTextFontSizeProperty, value);
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ApplyAll()
    {
        ApplyText();
        ApplyBackground();
        ApplyForeground();
        ApplyCornerRadius();
        ApplyPadding();
        ApplyFontSize();
    }

    private void ApplyText()
    {
        _badgeTextBlock.Text = BadgeText ?? string.Empty;
    }

    private void ApplyBackground()
    {
        _badgeBorder.Background = BadgeBackground;
    }

    private void ApplyForeground()
    {
        _badgeTextBlock.Foreground = BadgeForeground;
    }

    private void ApplyCornerRadius()
    {
        _badgeBorder.CornerRadius = BadgeCornerRadius;
    }

    private void ApplyPadding()
    {
        _badgeBorder.Padding = BadgePadding;
    }

    private void ApplyFontSize()
    {
        _badgeTextBlock.FontSize = BadgeTextFontSize;
    }
}