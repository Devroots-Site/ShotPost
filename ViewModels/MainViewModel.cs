using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ShotPost.Services;

namespace ShotPost.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly HomeViewModel _home = new();
    private readonly SettingsViewModel _settings;

    public MainViewModel() : this(new SettingsService())
    {
    }

    public MainViewModel(SettingsService settingsService)
    {
        _settings = new SettingsViewModel(settingsService);
    }

    /// <summary>
    /// The view model currently shown in the main window.
    /// The ViewLocator resolves the matching view (e.g. LoadingViewModel -> LoadingView).
    /// </summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNavigationVisible), nameof(IsHomeActive), nameof(IsSettingsActive))]
    public partial ViewModelBase CurrentView { get; set; } = new LoadingViewModel();

    /// <summary>Die Seitenleiste bleibt waehrend des Ladebildschirms ausgeblendet.</summary>
    public bool IsNavigationVisible => CurrentView is not LoadingViewModel;

    public bool IsHomeActive => CurrentView is HomeViewModel;

    public bool IsSettingsActive => CurrentView is SettingsViewModel;

    /// <summary>Zeigt kurz den Ladebildschirm und wechselt dann zur Startseite.</summary>
    public async Task InitializeAsync()
    {
        await Task.Delay(TimeSpan.FromSeconds(1.2));
        CurrentView = _home;
    }

    [RelayCommand]
    private void ShowHome() => CurrentView = _home;

    [RelayCommand]
    private void ShowSettings() => CurrentView = _settings;
}
