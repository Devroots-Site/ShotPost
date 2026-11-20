using System;
using System.IO;
using System.Text.Json;
using ShotPost.Models;

namespace ShotPost.Services;

/// <summary>Laedt und speichert die Einstellungen unter %AppData%\ShotPost\settings.json.</summary>
public sealed class SettingsService
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public string FilePath { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ShotPost",
        "settings.json");

    /// <summary>Liefert die gespeicherten Einstellungen; bei fehlender oder defekter Datei die Standardwerte.</summary>
    public AppSettings Load()
    {
        try
        {
            if (!File.Exists(FilePath))
                return new AppSettings();

            return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(FilePath), JsonOptions)
                   ?? new AppSettings();
        }
        catch (Exception ex) when (ex is IOException or JsonException or UnauthorizedAccessException)
        {
            return new AppSettings();
        }
    }

    public void Save(AppSettings settings)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(FilePath)!);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, JsonOptions));
    }
}
