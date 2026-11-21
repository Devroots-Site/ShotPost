# Oberfläche & Navigation

## Seiten

| ViewModel | View | Aufgabe |
| --- | --- | --- |
| `MainViewModel` | `MainWindow` | Fenster mit Navigation, hält die aktuelle Seite |
| `LoadingViewModel` | `LoadingView` | Ladebildschirm beim Start |
| `HomeViewModel` | `HomeView` | Startseite |
| `DatabaseViewModel` | `DatabaseView` | Tabellen der SSMDB2 ansehen |
| `SettingsViewModel` | `SettingsView` | Einstellungen in fünf Reitern |

## Navigation

`MainViewModel.CurrentView` enthält das ViewModel der aktuellen Seite. Das `MainWindow` bindet
ein `ContentControl` daran; der [`ViewLocator`](/framework/#viewlocator) erzeugt die passende View.

```csharp
[RelayCommand]
private void ShowSettings() => CurrentView = _settings;
```

- Die ViewModels der Seiten werden **einmal** erzeugt und wiederverwendet – ihr Zustand bleibt
  beim Wechsel erhalten.
- `IsHomeActive`, `IsDatabaseActive`, `IsSettingsActive` markieren den aktiven Eintrag.
- Solange der Ladebildschirm läuft, ist die Navigation ausgeblendet (`IsNavigationVisible`).
- `ShowDatabase` lädt beim Öffnen sofort den aktuellen Stand (`DatabaseViewModel.LoadAsync`).

### Sprung in die Datenbank-Einstellungen

Fehlen Zugangsdaten oder schlägt die Verbindung fehl, zeigt die Datenbank-Ansicht einen Button zu
den Einstellungen. Dafür bekommt `DatabaseViewModel` eine `Action` übergeben, die den Reiter
„Datenbank" auswählt.

::: warning Reiter-Reihenfolge
`SettingsViewModel.DatabaseTabIndex` (aktuell `4`) muss zur Position des Reiters im `TabControl`
von `SettingsView.axaml` passen. Wer Reiter umsortiert, passt die Konstante an.
:::

## Datenbank-Ansicht

`DatabaseViewModel.LoadAsync` ist der zentrale Ablauf:

- bricht einen noch laufenden Ladevorgang ab (`CancellationTokenSource`),
- prüft, ob Host, Benutzer und Datenbankname gesetzt sind (`IsConfigured`),
- lädt eine Seite über [`SsmdbReader.ReadPageAsync`](./datenzugriff) mit 30 s Zeitlimit,
- springt auf die letzte vorhandene Seite, falls die Tabelle inzwischen geschrumpft ist.

Seitengrößen: 100, 1000 oder alle. Ein Wechsel von Tabelle oder Seitengröße lädt ab Seite 1 neu.

## Responsives Layout

`MainWindow.axaml.cs` setzt unter **760 px** Fensterbreite die Klasse `compact` am Fenster.
Styles mit `Window.compact …` verschieben dann die Navigation nach oben und verkleinern Abstände.

## Gemeinsame Styles

In `App.axaml` definiert und in allen Views nutzbar:

| Klasse | Wirkung |
| --- | --- |
| `pageTitle` | Seitenüberschrift |
| `pageHeader`, `pageSide`, `tabPage`, `footer` | einheitliche Seitenabstände |
| `primary` (Button) | Hauptaktion in der Hauptfarbe |

::: tip Farben immer als DynamicResource
Die `Brand…`-Ressourcen werden zur Laufzeit vom [`ThemeService`](./einstellungen#theme)
überschrieben. Mit `StaticResource` würde eine View die Farbänderung nicht mitbekommen.
:::
