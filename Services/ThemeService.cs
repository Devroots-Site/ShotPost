using System;
using Avalonia;
using Avalonia.Media;
using ShotPost.Models;

namespace ShotPost.Services;

/// <summary>Leitet aus der Hauptfarbe die Brand-Ressourcen ab und setzt sie zur Laufzeit.</summary>
public static class ThemeService
{
    public static Color DefaultPrimary { get; } = Color.Parse(AppSettings.DefaultPrimaryColor);

    public static Color ParseOrDefault(string? hex)
        => Color.TryParse(hex, out var color) ? color : DefaultPrimary;

    public static string ToHex(Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

    public static void ApplyPrimaryColor(Color primary)
    {
        var resources = Application.Current?.Resources;
        if (resources is null)
            return;

        primary = Color.FromRgb(primary.R, primary.G, primary.B); // Alpha ignorieren
        var dark = Mix(primary, Colors.Black, 0.45);
        var light = Mix(primary, Colors.White, 0.18);

        resources["BrandPrimary"] = primary;
        resources["BrandPrimaryDark"] = dark;
        resources["BrandPrimaryLight"] = light;

        resources["BrandPrimaryBrush"] = new SolidColorBrush(primary);
        resources["BrandPrimaryHoverBrush"] = new SolidColorBrush(light);
        resources["BrandPrimaryPressedBrush"] = new SolidColorBrush(dark);
        resources["BrandSubtleBrush"] = new SolidColorBrush(Mix(primary, Colors.White, 0.82));

        resources["BrandBackgroundBrush"] = new LinearGradientBrush
        {
            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
            EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
            GradientStops =
            {
                new GradientStop(dark, 0),
                new GradientStop(primary, 0.55),
                new GradientStop(light, 1),
            },
        };
    }

    private static Color Mix(Color from, Color to, double amount)
    {
        byte Channel(byte a, byte b) => (byte)Math.Round(a + (b - a) * amount);
        return Color.FromRgb(Channel(from.R, to.R), Channel(from.G, to.G), Channel(from.B, to.B));
    }
}
