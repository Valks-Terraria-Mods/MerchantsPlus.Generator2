using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MerchantsPlus.Generator;

public partial class CatalogItemRowView : UserControl
{
    public CatalogItemRowView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}