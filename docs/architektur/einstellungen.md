# Einstellungen & Theme

## Speicherung

`SettingsService` liest und schreibt `AppSettings` als JSON:

```text
%AppData%\ShotPost\settings.json
```

- `Load()` liefert bei fehlender oder defekter Datei die **Standardwerte** – es wirft nicht.
- `Save()` legt den Ordner bei Bedarf an.

::: warning Passwort im Klartext
Das Datenbank-Passwort steht unverschlüsselt in der Datei.
:::

## Inhalt von AppSettings

| Gruppe | Properties |
| --- | --- |
| Zielserver | `ServerUrl` |
| Welche Tabellen | `SendScheiben`, `SendSerien`, `SendTreffer`, `SendProbeschuesse` |
| Welche Felder | `ScheibenFelder`, `SerienFelder`, `TrefferFelder` (`null` = alle) |
| Darstellung | `PrimaryColor` (Standard `#2563EB`) |
| Synchronisation | `SyncIntervalMinutes` (Standard 5) |
| Datenbank | `DbHost`, `DbPort`, `DbUser`, `DbPassword`, `DbName` |

## SettingsViewModel

Die Einstellungsseite arbeitet auf einer **Kopie**: Änderungen landen erst mit „Speichern" in der
Datei.

| Methode | Aufgabe |
| --- | --- |
| `Apply(AppSettings)` | Werte aus der Datei in die Oberfläche übernehmen |
| `Save` | prüfen (URL-Schema, Port 1–65535) und speichern |
| `Reset` | ungespeicherte Änderungen verwerfen |
| `TestDatabaseAsync` | Verbindung mit den **eingegebenen** Werten testen (10 s Zeitlimit) |

Die Reiter der Seite: Server · Daten · Darstellung · Synchronisation · Datenbank.

### Eine neue Einstellung hinzufügen

1. Property mit Standardwert in `AppSettings`
2. `[ObservableProperty]` im `SettingsViewModel`
3. In `Apply` (laden) **und** `Save` (speichern) ergänzen
4. Eingabefeld im passenden Reiter von `SettingsView.axaml`

Alte `settings.json`-Dateien bleiben lesbar: fehlende Werte erhalten den Standardwert.

## Theme

`ThemeService.ApplyPrimaryColor` leitet aus der Hauptfarbe alle `Brand…`-Ressourcen ab
(dunklere und hellere Variante, Verlauf für die Navigation) und setzt sie in
`Application.Current.Resources`.

- Beim Start wendet `App` die gespeicherte Farbe an.
- In den Einstellungen wirkt eine Farbänderung **sofort als Vorschau**; dauerhaft wird sie erst
  mit „Speichern".
- Die Standardwerte der Ressourcen stehen in `App.axaml`.

Neue Ressourcen, die der Hauptfarbe folgen sollen, müssen an beiden Stellen ergänzt werden:
Standardwert in `App.axaml`, Berechnung im `ThemeService`.
