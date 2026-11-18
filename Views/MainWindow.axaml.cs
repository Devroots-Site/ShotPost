using Avalonia;
using Avalonia.Controls;

namespace ShotPost.Views;

public partial class MainWindow : Window
{
    /// <summary>Unterhalb dieser Fensterbreite wandert die Navigation nach oben (Klasse "compact").</summary>
    private const double CompactBreakpoint = 760;

    public MainWindow()
    {
        InitializeComponent();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ClientSizeProperty)
            Classes.Set("compact", ClientSize.Width < CompactBreakpoint);
    }
}
