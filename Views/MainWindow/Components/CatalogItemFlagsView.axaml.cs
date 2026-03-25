using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MerchantsPlus.Generator;

public partial class CatalogItemFlagsView : UserControl
{
    public CatalogItemFlagsView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}