# Überblick

ShotPost ist eine Desktop-App, die Schießergebnisse aus einer **Meyton-Datenbank (SSMDB2)** liest
und an einen frei einstellbaren Server überträgt.

## Aktueller Stand

| Bereich | Status |
| --- | --- |
| Startseite, Ladebildschirm, Navigation | vorhanden |
| Einstellungen (Server, Daten, Darstellung, Synchronisation, Datenbank) | vorhanden |
| Datenbank-Ansicht (nur lesend, mit Seitenwechsel) | vorhanden |
| Payload-Vorschau in den Einstellungen | vorhanden |
| **Synchronisation zum Server** | **noch nicht implementiert** |

::: warning Sync fehlt noch
Server-Adresse, Intervall und Feldauswahl werden bereits gespeichert, aber es wird noch nichts
gesendet. Der [`PayloadBuilder`](/architektur/payload) wird bisher nur für die Vorschau genutzt.
:::

## Technik

| Thema | Eingesetzt |
| --- | --- |
| Sprache / Laufzeit | C# auf .NET 10 (`global.json` legt das SDK fest) |
| Oberfläche | [Avalonia UI](https://docs.avaloniaui.net/) 12 mit Fluent-Theme |
| MVVM | [CommunityToolkit.Mvvm](https://learn.microsoft.com/dotnet/communitytoolkit/mvvm/) |
| Datenbank | [MySqlConnector](https://mysqlconnector.net/) gegen MariaDB/MySQL, Mapping mit [Dapper](https://www.learndapper.com/) |
| Formatierung | [CSharpier](https://csharpier.com/) als lokales dotnet-Tool |
| CI | GitHub Actions (`.github/workflows/ci.yml`) |

## Wo weiterlesen?

- Erste Schritte: [Schnellstart](./schnellstart)
- Wo liegt was: [Projektstruktur](./projektstruktur)
- Vor dem ersten Commit: [Entwicklungs-Workflow](./workflow)
- Neu in Avalonia: [Framework](/framework/)
