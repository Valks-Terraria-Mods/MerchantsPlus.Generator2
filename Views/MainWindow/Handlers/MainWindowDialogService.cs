using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace MerchantsPlus.Generator;

public sealed class MainWindowDialogService{
    private readonly Window _owner;

    public MainWindowDialogService(Window owner)
    {
        _owner = owner;
    }

    public async Task<string?> PromptForNameAsync(string title, string prompt)
    {
        var textBox = new TextBox
        {
            Width = 340,
            Watermark = "Name"
        };

        string? result = null;
        var cancelButton = new Button { Content = "Cancel", MinWidth = 80 };
        var okButton = new Button { Content = "OK", MinWidth = 80 };

        var dialog = new Window
        {
            Title = title,
            Width = 420,
            Height = 180,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(16),
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = prompt },
                    textBox,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 10,
                        Children =
                        {
                            cancelButton,
                            okButton
                        }
                    }
                }
            }
        };

        cancelButton.Click += (_, _) => dialog.Close();
        okButton.Click += (_, _) =>
        {
            result = textBox.Text?.Trim();
            dialog.Close();
        };

        await dialog.ShowDialog(_owner);
        return result;
    }

    public async Task<ShopMoveDestination?> PromptForShopMoveDestinationAsync(
        string title,
        string prompt,
        string destinationLabel,
        IReadOnlyList<ShopMoveDestination> destinations)
    {
        if (destinations.Count == 0)
            return null;

        var destinationPicker = new ComboBox
        {
            Width = 340,
            ItemsSource = destinations,
            SelectedIndex = 0
        };

        ShopMoveDestination? result = null;
        var cancelButton = new Button { Content = "Cancel", MinWidth = 80 };
        var moveButton = new Button { Content = "Move", MinWidth = 80 };

        var dialog = new Window
        {
            Title = title,
            Width = 420,
            Height = 220,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
            Content = new StackPanel
            {
                Margin = new Thickness(16),
                Spacing = 12,
                Children =
                {
                    new TextBlock { Text = prompt },
                    new TextBlock { Text = destinationLabel },
                    destinationPicker,
                    new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Right,
                        Spacing = 10,
                        Children =
                        {
                            cancelButton,
                            moveButton
                        }
                    }
                }
            }
        };

        cancelButton.Click += (_, _) => dialog.Close();
        moveButton.Click += (_, _) =>
        {
            result = destinationPicker.SelectedItem as ShopMoveDestination;
            dialog.Close();
        };

        await dialog.ShowDialog(_owner);
        return result;
    }
}
