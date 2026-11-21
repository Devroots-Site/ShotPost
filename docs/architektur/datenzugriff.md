# Datenzugriff (SSMDB2)

Alles unter `Data/`. Der Zugriff ist **ausschließlich lesend**.

## Ablauf einer Abfrage

```text
SsmdbReader.ReadPageAsync(settings, tabelle, limit, offset)
  1. Verbindung öffnen            (MySqlConnector, Timeout 5 s)
  2. SsmdbSchemaDetector          → welche Spalten gibt es?   → SsmdbSchema
  3. SsmdbQueries                 → SELECT mit festen Spaltennamen
  4. Dapper (QueryAsync<T>)       → Zeile → Scheibe | Serie | Treffer
  5. Ergebnis: SsmdbPage(Rows, TotalCount, VersionLabel)
```

## Die Klassen

| Klasse | Aufgabe |
| --- | --- |
| `SsmdbReader` | Einstieg: baut die Verbindung, liest eine Seite einer Tabelle (per Dapper) |
| `SsmdbSchemaDetector` | liest vorhandene Spalten aus `information_schema.COLUMNS` |
| `SsmdbSchema` | beantwortet `Has(tabelle, spalte)` und liefert ein `VersionLabel` |
| `SsmdbQueries` | baut die `SELECT`-Statements aus festen Spaltenlisten (`SsmdbColumn`) |

## Mapping mit Dapper

[Dapper](https://www.learndapper.com/) ordnet jede Ergebnisspalte der gleichnamigen Property im
Modell zu (Groß-/Kleinschreibung egal, Reihenfolge egal). Unterschiedliche Zahlentypen je Version
(z.B. `SportpassID` als `INT` oder `BIGINT`) konvertiert Dapper selbst.

::: info Warum kein EF Core?
Die SSMDB2 gehört Meyton: Wir lesen nur und spielen keine Migrations ein. Außerdem fehlen je nach
Version Spalten, ein festes EF-Modell würde dort fehlschlagen. Dapper mit selbst gebautem `SELECT`
deckt das mit wenig Aufwand ab.
:::

## Umgang mit Schemaversionen

Die Versionsnummer steht nicht in der Datenbank. Deshalb wird geprüft, **welche Spalten
existieren**:

| Spalte | vorhanden ab |
| --- | --- |
| `Scheiben.StandNrText` | V8 |
| `Treffer.Serie` | V8 |
| `Scheiben.BesterTeiler001` | V9 |

Fehlt eine Spalte, schreibt `SsmdbQueries` stattdessen `NULL AS Spalte` ins `SELECT`. Dadurch
liefert jede Version **dieselben Spaltennamen**, und dieselben Modelle passen für alle Versionen.

```sql
-- V5: StandNrText fehlt
SELECT `ScheibenID`, …, NULL AS `StandNrText`, … FROM `Scheiben` ORDER BY ScheibenID
```

`VersionLabel` ist nur eine grobe Einordnung für die Anzeige: `V5/V7`, `V8` oder `V9 oder neuer`
(V5 und V7 lassen sich an den Spalten nicht unterscheiden).

## Eine Spalte ergänzen

Zwei Stellen:

1. Property im Modell (`Models/`)
2. Spaltenname in der Liste in `SsmdbQueries` (`ScheibenColumns`, `SerienColumns`, `TrefferColumns`)

Heißt die Property anders als die Spalte, einen Alias angeben:

```csharp
new("Serie", "SerieNr"),   // SELECT `Serie` AS `SerieNr`
```

Gibt es die Spalte erst ab einer bestimmten Version, die Property nullable machen und die Version
in die Tabelle oben eintragen.

## Sicherheit

- Tabellen- und Spaltennamen kommen aus festen Listen, nie aus Nutzereingaben.
- Werte (`LIMIT`, `OFFSET`, Datenbankname) gehen als Parameter.
- Fehlt eine der drei Tabellen, wirft der Detector eine `SsmdbSchemaException`
  („Ist das eine SSMDB2?").

Weiterführend: [MySqlConnector-Dokumentation](https://mysqlconnector.net/),
[Dapper-Dokumentation](https://www.learndapper.com/)
