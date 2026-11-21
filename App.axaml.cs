using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ShotPost.Services;
using ShotPost.ViewModels;
using ShotPost.Views;

namespace ShotPost;

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
            var settingsService = new SettingsService();
            ThemeService.ApplyPrimaryColor(
                ThemeService.ParseOrDefault(settingsService.Load().PrimaryColor)
            );

            var mainViewModel = new MainViewModel(settingsService);
            desktop.MainWindow = new MainWindow { DataContext = mainViewModel };
            _ = mainViewModel.InitializeAsync();
        }

        base.OnFrameworkInitializationCompleted();
    }
}
