using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace MerchantsPlus.Generator;

public class App : Application
{
    private readonly ApplicationStartupCoordinator _startupCoordinator;

    public App()
        : this(new ApplicationStartupCoordinator(new CatalogDataBootstrapper(), new MainWindowFactory()))
    {
    }

    internal App(ApplicationStartupCoordinator startupCoordinator)
    {
        _startupCoordinator = startupCoordinator;
    }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = _startupCoordinator.CreateMainWindow();

        base.OnFrameworkInitializationCompleted();
    }
}
