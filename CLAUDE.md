# ShotPost

Avalonia-Desktop-App (.NET 10, MVVM mit CommunityToolkit.Mvvm): liest Meyton-Ergebnisse aus der
SSMDB2 (nur lesend) und soll sie an einen Server senden. Der Sync ist noch nicht implementiert.

## Befehle

- Formatieren: `dotnet csharpier format .` (CI prueft mit `dotnet csharpier check .`)
- Bauen: `dotnet build` – in der CI (`CI=true`) sind Warnungen Fehler
- Testdatenbanken: `docker compose up -d` (Ports 3306–3309, siehe `docker-compose.yml`)
- Doku: `cd docs && npm install && npm run dev` / `npm run build`

## Konventionen

- Code-Kommentare deutsch, ohne Umlaute. Doku-Texte unter `docs/` deutsch mit Umlauten.
- `XyzViewModel` gehoert zu `Views/XyzView` (ViewLocator arbeitet ueber den Namen).
- Brand-Farben in XAML immer als `DynamicResource`.

## Dokumentation

Die Entwickler-Doku liegt unter `docs/` (VitePress). **Aendert sich Code, wird die passende Seite
im selben Arbeitsschritt mit aktualisiert.** Ein Stop-Hook (`.claude/hooks/docs-sync.mjs`) erinnert
daran, falls Code ohne Doku-Aenderung bearbeitet wurde.

| Geaenderter Code | Doku-Seite |
| --- | --- |
| `Data/` | `docs/architektur/datenzugriff.md` |
| `Models/Scheibe.cs`, `Serie.cs`, `Treffer.cs` | `docs/architektur/modelle.md` |
| `Models/AppSettings.cs`, `Services/SettingsService.cs`, `Services/ThemeService.cs`, `ViewModels/SettingsViewModel.cs` | `docs/architektur/einstellungen.md` |
| `Services/PayloadBuilder.cs`, `ViewModels/DataTableViewModel.cs` | `docs/architektur/payload.md` |
| `Views/`, uebrige `ViewModels/`, `App.axaml`, `ViewLocator.cs` | `docs/architektur/oberflaeche.md` |
| `Program.cs`, `App.axaml.cs`, neue Schichten/Ordner | `docs/architektur/index.md`, `docs/einstieg/projektstruktur.md` |
| `docker-compose.yml`, `Database/`, `global.json` | `docs/einstieg/schnellstart.md` |
| `.github/workflows/`, `.editorconfig`, `ShotPost.csproj`, `dotnet-tools.json` | `docs/einstieg/workflow.md` |
| Neue Features / Status (z.B. Sync) | `docs/einstieg/index.md` |
| Neue Avalonia-/Toolkit-Muster | `docs/framework/` |

Regeln fuer die Doku:

- Kurz halten: zum schnellen Nachschlagen, keine vollstaendige API-Referenz. Fuer Details auf die
  offiziellen Dokus verlinken.
- Nur beschreiben, was im Code tatsaechlich so ist; veraltete Aussagen entfernen statt ergaenzen.
- Neue Seiten in `docs/.vitepress/config.mts` (Sidebar) eintragen.
- Nach Doku-Aenderungen `npm run build` in `docs/` ausfuehren (findet tote Links).
