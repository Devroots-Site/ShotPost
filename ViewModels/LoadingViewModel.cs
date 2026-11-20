using CommunityToolkit.Mvvm.ComponentModel;

namespace ShotPost.ViewModels;

public partial class LoadingViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Title { get; set; } = "ShootPost";

    [ObservableProperty]
    public partial string Subtitle { get; set; } = "Deine Meyton-Ergebnisse, synchronisiert mit deinem Server";

    [ObservableProperty]
    public partial string StatusText { get; set; } = "Wird geladen…";
}
