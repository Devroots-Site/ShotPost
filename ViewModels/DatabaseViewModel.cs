using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MySqlConnector;
using ShotPost.Data;
using ShotPost.Services;

namespace ShotPost.ViewModels;

/// <param name="Size">Zeilen pro Seite; <c>null</c> = alle.</param>
/// <param name="Label">Anzeigetext.</param>
public sealed record PageSizeOption(int? Size, string Label);

/// <summary>
/// Zeigt die Eintraege der in den Einstellungen hinterlegten Meyton-Datenbank.
/// Nur lesend – zum Nachsehen, kein Abfragewerkzeug.
/// </summary>
public partial class DatabaseViewModel : ViewModelBase
{
    private readonly SettingsService _settingsService;
    private readonly Action _openSettings;
    private CancellationTokenSource? _loadCts;
    private bool _activated;

    public DatabaseViewModel() : this(new SettingsService(), () => { })
    {
    }

    /// <param name="settingsService">Quelle der gespeicherten Zugangsdaten.</param>
    /// <param name="openSettings">Oeffnet die Einstellungen auf der Unterseite "Datenbank".</param>
    public DatabaseViewModel(SettingsService settingsService, Action openSettings)
    {
        _settingsService = settingsService;
        _openSettings = openSettings;
        SelectedTable = Tables[0];
        SelectedPageSize = PageSizeOptions[0];
    }

    public IReadOnlyList<string> Tables { get; } =
        [SsmdbSchema.TabelleScheiben, SsmdbSchema.TabelleSerien, SsmdbSchema.TabelleTreffer];

    public IReadOnlyList<PageSizeOption> PageSizeOptions { get; } =
    [
        new(100, "100"),
        new(1000, "1000"),
        new(null, "Alle"),
    ];

    [ObservableProperty]
    public partial string SelectedTable { get; set; }

    [ObservableProperty]
    public partial PageSizeOption SelectedPageSize { get; set; }

    /// <summary>Zeilen der aktuellen Seite (List&lt;Scheibe|Serie|Treffer&gt;).</summary>
    [ObservableProperty]
    public partial IEnumerable? Rows { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowSetupPrompt), nameof(ShowData))]
    public partial bool IsConfigured { get; set; } = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError), nameof(ShowSetupPrompt), nameof(ShowData))]
    public partial string ErrorMessage { get; set; } = "";

    [ObservableProperty]
    public partial bool IsLoading { get; set; }

    [ObservableProperty]
    public partial string ConnectionText { get; set; } = "";

    [ObservableProperty]
    public partial string PageText { get; set; } = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand), nameof(NextPageCommand))]
    public partial int PageIndex { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    public partial long TotalCount { get; set; }

    public bool HasError => ErrorMessage.Length > 0;

    /// <summary>Keine Zugangsdaten oder Verbindung fehlgeschlagen: Hinweis mit Button zu den Einstellungen.</summary>
    public bool ShowSetupPrompt => !IsConfigured || HasError;

    public bool ShowData => !ShowSetupPrompt;

    private int TotalPages =>
        SelectedPageSize.Size is { } size ? (int)Math.Max(1, (TotalCount + size - 1) / size) : 1;

    /// <summary>Laedt die aktuelle Seite neu aus der Datenbank.</summary>
    [RelayCommand(AllowConcurrentExecutions = true)]
    private Task RefreshAsync() => LoadAsync();

    [RelayCommand(CanExecute = nameof(CanGoToPreviousPage))]
    private Task PreviousPageAsync()
    {
        PageIndex--;
        return LoadAsync();
    }

    [RelayCommand(CanExecute = nameof(CanGoToNextPage))]
    private Task NextPageAsync()
    {
        PageIndex++;
        return LoadAsync();
    }

    [RelayCommand]
    private void OpenSettings() => _openSettings();

    private bool CanGoToPreviousPage() => PageIndex > 0;

    private bool CanGoToNextPage() => PageIndex < TotalPages - 1;

    partial void OnSelectedTableChanged(string value) => ReloadFromFirstPage();

    partial void OnSelectedPageSizeChanged(PageSizeOption value) => ReloadFromFirstPage();

    private void ReloadFromFirstPage()
    {
        if (!_activated)
            return;

        PageIndex = 0;
        _ = LoadAsync();
    }

    /// <summary>Liest die gespeicherten Zugangsdaten und laedt die aktuelle Seite. Bricht einen laufenden Ladevorgang ab.</summary>
    public async Task LoadAsync()
    {
        _activated = true;
        _loadCts?.Cancel();
        var cts = _loadCts = new CancellationTokenSource();

        var settings = _settingsService.Load();
        IsConfigured = !string.IsNullOrWhiteSpace(settings.DbHost)
                       && !string.IsNullOrWhiteSpace(settings.DbUser)
                       && !string.IsNullOrWhiteSpace(settings.DbName);
        if (!IsConfigured)
        {
            ErrorMessage = "";
            Rows = null;
            ConnectionText = "";
            return;
        }

        IsLoading = true;
        try
        {
            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cts.Token);
            timeout.CancelAfter(TimeSpan.FromSeconds(30));

            var size = SelectedPageSize.Size;
            var page = await SsmdbReader.ReadPageAsync(
                settings, SelectedTable, size, (long)PageIndex * (size ?? 0), timeout.Token);

            // Die Tabelle ist seit dem letzten Laden geschrumpft: auf die letzte vorhandene Seite springen.
            if (size is { } s && page.Rows.Count == 0 && page.TotalCount > 0 && PageIndex > 0)
            {
                PageIndex = (int)((page.TotalCount - 1) / s);
                page = await SsmdbReader.ReadPageAsync(
                    settings, SelectedTable, size, (long)PageIndex * s, timeout.Token);
            }

            if (cts.IsCancellationRequested)
                return;

            ErrorMessage = "";
            TotalCount = page.TotalCount;
            Rows = page.Rows;
            ConnectionText =
                $"{settings.DbName.Trim()} @ {settings.DbHost.Trim()}:{settings.DbPort} · SSMDB2 {page.VersionLabel} · Stand {DateTime.Now:HH:mm:ss}";
            PageText = $"Seite {PageIndex + 1} von {TotalPages} · {TotalCount} {(TotalCount == 1 ? "Eintrag" : "Einträge")}";
            NextPageCommand.NotifyCanExecuteChanged();
        }
        catch (OperationCanceledException) when (cts.IsCancellationRequested)
        {
            // Von einem neueren Ladevorgang abgeloest.
        }
        catch (OperationCanceledException)
        {
            ShowError("Zeitüberschreitung beim Laden der Daten.");
        }
        catch (Exception ex) when (ex is MySqlException or SsmdbSchemaException or InvalidOperationException)
        {
            ShowError($"Verbindung fehlgeschlagen: {ex.Message}");
        }
        finally
        {
            if (_loadCts == cts)
                IsLoading = false;
        }
    }

    private void ShowError(string message)
    {
        Rows = null;
        ConnectionText = "";
        ErrorMessage = message;
    }
}
