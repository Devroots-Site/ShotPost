# Schnellstart

## Voraussetzungen

- **.NET SDK 10** (`global.json` verlangt `10.0.100` oder ein neueres Feature-Band)
- **Docker** für die Testdatenbanken
- **Node.js** – nur für diese Dokumentation

## App starten

```bash
git clone https://github.com/Devroots-Site/ShotPost.git
cd ShotPost
dotnet tool restore      # CSharpier
dotnet run
```

## Testdatenbanken

`docker-compose.yml` startet je SSMDB2-Version einen MariaDB-Container, befüllt aus `Database/*.sql`.

```bash
docker compose up -d                           # starten
docker compose down                            # stoppen
docker compose down -v && docker compose up -d # Daten neu laden
```

| Version | Port | Datenbankname |
| --- | --- | --- |
| V5 | 3306 | `ssmdb2_v5` |
| V7 | 3307 | `ssmdb2_v7` |
| V8 | 3308 | `ssmdb2_v8` |
| V9 | 3309 | `ssmdb2_v9` |

Zugang überall: Host `localhost`, Benutzer `shotpost`, Passwort `shotpost`.
Die Container sind nur lokal erreichbar (`127.0.0.1`).

## Verbindung in der App eintragen

1. App starten → **Einstellungen** → Reiter **Datenbank**
2. Host, Port, Benutzer, Passwort und Datenbankname aus der Tabelle eintragen
3. **Verbindung testen** – die erkannte SSMDB2-Version wird angezeigt
4. **Speichern**, dann links **Datenbank** öffnen

::: tip Wo liegen die Einstellungen?
In `%AppData%\ShotPost\settings.json`. Datei löschen = Standardwerte.
Mehr dazu unter [Einstellungen & Theme](/architektur/einstellungen).
:::
