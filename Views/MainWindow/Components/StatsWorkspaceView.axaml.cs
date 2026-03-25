using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace MerchantsPlus.Generator;

public partial class StatsWorkspaceView : UserControl
{
    public StatsWorkspaceView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}