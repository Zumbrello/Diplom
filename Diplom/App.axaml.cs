using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;

namespace Diplom;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainWindow = new MainWindow
            {
                WindowState = WindowState.FullScreen, // Установите состояние окна в FullScreen
                CanResize = false // Запретить изменение размера окна
            };
            desktop.MainWindow = mainWindow;
            base.OnFrameworkInitializationCompleted();
        }

        base.OnFrameworkInitializationCompleted();
    }
}