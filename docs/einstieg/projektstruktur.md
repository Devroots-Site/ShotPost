# Projektstruktur

Ein einzelnes Projekt (`ShotPost.csproj`), Ordner = Namespace (`ShotPost.Data`, `ShotPost.Views`, …).

```text
ShotPost/
├─ Program.cs            Einstiegspunkt, konfiguriert Avalonia
├─ App.axaml(.cs)        Globale Ressourcen/Styles, baut MainWindow + MainViewModel
├─ ViewLocator.cs        Findet zur ViewModel-Klasse die passende View
├─ Views/                Oberfläche (XAML) – eine View je Seite
├─ ViewModels/           Zustand und Befehle der Seiten
├─ Models/               Datenklassen: Scheibe, Serie, Treffer, AppSettings
├─ Data/                 Lesender Zugriff auf die SSMDB2
├─ Services/             Einstellungen, Theme, Payload
├─ Database/             SQL-Dumps der Testdatenbanken (V5, V7, V8, V9)
├─ docker-compose.yml    Testdatenbanken
├─ docs/                 Diese Dokumentation (VitePress)
└─ .github/workflows/    CI-Pipeline
```

## Was gehört wohin?

| Ich möchte … | Ort | Doku |
| --- | --- | --- |
| eine neue Seite bauen | `Views/` + `ViewModels/` | [Rezept: neue Seite](/framework/neue-seite) |
| Layout oder Styles ändern | `Views/*.axaml`, `App.axaml` | [Oberfläche & Navigation](/architektur/oberflaeche) |
| eine Spalte der SSMDB2 ergänzen | `Models/`, `Data/` | [Datenzugriff](/architektur/datenzugriff) |
| eine neue Einstellung speichern | `Models/AppSettings.cs`, `SettingsViewModel` | [Einstellungen & Theme](/architektur/einstellungen) |
| ändern, was gesendet wird | `Services/PayloadBuilder.cs` | [Payload](/architektur/payload) |

## Konfigurationsdateien

| Datei | Zweck |
| --- | --- |
| `global.json` | legt die .NET-SDK-Version fest |
| `dotnet-tools.json` | lokale Tools (CSharpier) |
| `.editorconfig` | Stilregeln; werden im Build geprüft |
| `ShotPost.csproj` | Pakete, Analyzer, `TreatWarningsAsErrors` in der CI |
