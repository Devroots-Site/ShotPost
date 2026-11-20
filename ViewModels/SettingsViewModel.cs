using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySqlConnector;
using ShotPost.Data;
using ShotPost.Models;
using ShotPost.Services;

namespace ShotPost.ViewModels;

public sealed record SyncIntervalOption(int Minutes, string Label);

public partial class SettingsViewModel : ViewModelBase
{
    private static readonly JsonSerializerOptions PreviewJsonOptions = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly SettingsService _settingsService;

    public SettingsViewModel() : this(new SettingsService())
    {
    }

    public SettingsViewModel(SettingsService settingsService)
    {
        _settingsService = settingsService;
        SelectedSyncInterval = SyncIntervalOptions[2];

        ScheibenTable = new DataTableViewModel("Scheiben", "Ein Ergebnis je Schütze und Scheibe.", typeof(Scheibe));
        SerienTable = new DataTableViewModel("Serien", "Summen je Serie.", typeof(Serie));
        TrefferTable = new DataTableViewModel("Treffer", "Jeder einzelne Schuss.", typeof(Treffer));
        Tables = [ScheibenTable, SerienTable, TrefferTable];
        foreach (var table in Tables)
            table.Changed += UpdatePayloadPreview;

        Apply(_settingsService.Load());
    }

    public IReadOnlyList<SyncIntervalOption> SyncIntervalOptions { get; } =
    [
        new(1, "Jede Minute"),
        new(2, "Alle 2 Minuten"),
        new(5, "Alle 5 Minuten"),
        new(10, "Alle 10 Minuten"),
        new(15, "Alle 15 Minuten"),
        new(30, "Alle 30 Minuten"),
        new(60, "Jede Stunde"),
    ];

    /// <summary>Position der Unterseite "Datenbank" im TabControl der SettingsView.</summary>
    public const int DatabaseTabIndex = 4;

    /// <summary>Aktuell gezeigte Unterseite.</summary>
    [ObservableProperty]
    public partial int SelectedTabIndex { get; set; }

    // Server
    [ObservableProperty]
    public partial string ServerUrl { get; set; } = "";

    // Datenauswahl
    public DataTableViewModel ScheibenTable { get; }
    public DataTableViewModel SerienTable { get; }
    public DataTableViewModel TrefferTable { get; }
    public IReadOnlyList<DataTableViewModel> Tables { get; }

    [ObservableProperty]
    public partial bool SendProbeschuesse { get; set; }

    /// <summary>Beispiel, wie die gesendeten Daten mit der aktuellen Auswahl aufgebaut sind.</summary>
    [ObservableProperty]
    public partial string PayloadPreview { get; set; } = "";

    // Darstellung
    [ObservableProperty]
    public partial Color PrimaryColor { get; set; }

    // Synchronisation
    [ObservableProperty]
    public partial SyncIntervalOption SelectedSyncInterval { get; set; }

    // Datenbank
    [ObservableProperty]
    public partial string DbHost { get; set; } = "";

    [ObservableProperty]
    public partial decimal? DbPort { get; set; }

    [ObservableProperty]
    public partial string DbUser { get; set; } = "";

    [ObservableProperty]
    public partial string DbPassword { get; set; } = "";

    [ObservableProperty]
    public partial string DbName { get; set; } = "";

    // Rueckmeldungen
    [ObservableProperty]
    public partial string StatusMessage { get; set; } = "";

    [ObservableProperty]
    public partial bool StatusIsError { get; set; }

    [ObservableProperty]
    public partial string DbTestMessage { get; set; } = "";

    [ObservableProperty]
    public partial bool DbTestIsError { get; set; }

    /// <summary>Farbaenderung sofort als Vorschau anwenden; dauerhaft erst nach "Speichern".</summary>
    partial void OnPrimaryColorChanged(Color value) => ThemeService.ApplyPrimaryColor(value);

    [RelayCommand]
    private void SelectPresetColor(string hex) => PrimaryColor = ThemeService.ParseOrDefault(hex);

    [RelayCommand]
    private void Save()
    {
        var url = ServerUrl.Trim();
        if (url.Length > 0 &&
            (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
             (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
        {
            SetStatus("Die Server-Adresse muss mit http:// oder https:// beginnen.", isError: true);
            return;
        }

        if (DbPort is not (>= 1 and <= 65535))
        {
            SetStatus("Der Datenbank-Port muss zwischen 1 und 65535 liegen.", isError: true);
            return;
        }

        try
        {
            _settingsService.Save(new AppSettings
            {
                ServerUrl = url,
                SendScheiben = ScheibenTable.IsEnabled,
                SendSerien = SerienTable.IsEnabled,
                SendTreffer = TrefferTable.IsEnabled,
                SendProbeschuesse = SendProbeschuesse,
                ScheibenFelder = ScheibenTable.SelectedFieldNames,
                SerienFelder = SerienTable.SelectedFieldNames,
                TrefferFelder = TrefferTable.SelectedFieldNames,
                PrimaryColor = ThemeService.ToHex(PrimaryColor),
                SyncIntervalMinutes = SelectedSyncInterval.Minutes,
                DbHost = DbHost.Trim(),
                DbPort = (int)DbPort.Value,
                DbUser = DbUser.Trim(),
                DbPassword = DbPassword,
                DbName = DbName.Trim(),
            });
            ServerUrl = url;
            SetStatus("Einstellungen gespeichert.", isError: false);
        }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
        {
            SetStatus($"Speichern fehlgeschlagen: {ex.Message}", isError: true);
        }
    }

    /// <summary>Verwirft ungespeicherte Aenderungen.</summary>
    [RelayCommand]
    private void Reset()
    {
        Apply(_settingsService.Load());
        DbTestMessage = "";
        SetStatus("Änderungen verworfen.", isError: false);
    }

    /// <summary>Prueft die eingegebenen Zugangsdaten. Nur lesender Zugriff, kein Sync.</summary>
    [RelayCommand]
    private async Task TestDatabaseAsync()
    {
        if (DbPort is not (>= 1 and <= 65535))
        {
            SetDbTest("Der Port muss zwischen 1 und 65535 liegen.", isError: true);
            return;
        }

        var dbName = DbName.Trim();
        var builder = SsmdbReader.BuildConnectionString(DbHost, (int)DbPort.Value, DbUser, DbPassword, dbName);

        SetDbTest("Verbindung wird geprüft…", isError: false);
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            await using var connection = new MySqlConnection(builder.ConnectionString);
            await connection.OpenAsync(cts.Token);
            var schema = await SsmdbSchemaDetector.DetectAsync(connection, dbName, cts.Token);
            SetDbTest($"Verbindung erfolgreich. Erkannte SSMDB2-Version: {schema.VersionLabel}.", isError: false);
        }
        catch (OperationCanceledException)
        {
            SetDbTest("Zeitüberschreitung beim Verbinden.", isError: true);
        }
        catch (Exception ex) when (ex is MySqlException or SsmdbSchemaException or InvalidOperationException)
        {
            SetDbTest($"Verbindung fehlgeschlagen: {ex.Message}", isError: true);
        }
    }

    private void Apply(AppSettings s)
    {
        ServerUrl = s.ServerUrl;
        ScheibenTable.Load(s.SendScheiben, s.ScheibenFelder);
        SerienTable.Load(s.SendSerien, s.SerienFelder);
        TrefferTable.Load(s.SendTreffer, s.TrefferFelder);
        SendProbeschuesse = s.SendProbeschuesse;
        PrimaryColor = ThemeService.ParseOrDefault(s.PrimaryColor);
        SelectedSyncInterval =
            SyncIntervalOptions.FirstOrDefault(o => o.Minutes == s.SyncIntervalMinutes) ?? SyncIntervalOptions[2];
        DbHost = s.DbHost;
        DbPort = s.DbPort;
        DbUser = s.DbUser;
        DbPassword = s.DbPassword;
        DbName = s.DbName;
        UpdatePayloadPreview();
    }

    /// <summary>Zeigt mit leeren Beispieldatensaetzen, welche Felder gesendet wuerden.</summary>
    private void UpdatePayloadPreview()
    {
        var payload = new Dictionary<string, object?>();
        if (ScheibenTable.IsEnabled)
            payload["Scheiben"] = new[] { PayloadBuilder.Build(new Scheibe(), ScheibenTable.SelectedFieldNames) };
        if (SerienTable.IsEnabled)
            payload["Serien"] = new[] { PayloadBuilder.Build(new Serie(), SerienTable.SelectedFieldNames) };
        if (TrefferTable.IsEnabled)
            payload["Treffer"] = new[] { PayloadBuilder.Build(new Treffer(), TrefferTable.SelectedFieldNames) };

        PayloadPreview = JsonSerializer.Serialize(payload, PreviewJsonOptions);
    }

    private void SetStatus(string message, bool isError)
    {
        StatusIsError = isError;
        StatusMessage = message;
    }

    private void SetDbTest(string message, bool isError)
    {
        DbTestIsError = isError;
        DbTestMessage = message;
    }
}
