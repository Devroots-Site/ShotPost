using System.Collections.Generic;

namespace ShotPost.Models;

/// <summary>Vom Benutzer einstellbare Werte. Wird als JSON gespeichert (siehe SettingsService).</summary>
public sealed class AppSettings
{
    public const string DefaultPrimaryColor = "#2563EB";

    // Zielserver
    public string ServerUrl { get; set; } = "";

    // Welche Tabellen an den Server gehen
    public bool SendScheiben { get; set; } = true;
    public bool SendSerien { get; set; } = true;
    public bool SendTreffer { get; set; } = true;
    public bool SendProbeschuesse { get; set; }

    // Welche Felder (Property-Namen der Modelle) je Tabelle gesendet werden. null = alle.
    public List<string>? ScheibenFelder { get; set; }
    public List<string>? SerienFelder { get; set; }
    public List<string>? TrefferFelder { get; set; }

    // Darstellung
    public string PrimaryColor { get; set; } = DefaultPrimaryColor;

    // Synchronisation (der Sync selbst ist noch nicht implementiert)
    public int SyncIntervalMinutes { get; set; } = 5;

    // Meyton-Datenbank (SSMDB2). Das Passwort liegt im Klartext in der Datei.
    public string DbHost { get; set; } = "localhost";
    public int DbPort { get; set; } = 3306;
    public string DbUser { get; set; } = "";
    public string DbPassword { get; set; } = "";
    public string DbName { get; set; } = "SSMDB2";
}
