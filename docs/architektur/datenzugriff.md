# Datenzugriff (SSMDB2)

Alles unter `Data/`. Der Zugriff ist **ausschließlich lesend**.

## Ablauf einer Abfrage

```text
SsmdbReader.ReadPageAsync(settings, tabelle, limit, offset)
  1. Verbindung öffnen            (MySqlConnector, Timeout 5 s)
  2. SsmdbSchemaDetector          → welche Spalten gibt es?   → SsmdbSchema
  3. SsmdbQueries                 → SELECT mit fester Spaltenreihenfolge
  4. SsmdbMapper                  → Zeile → Scheibe | Serie | Treffer
  5. Ergebnis: SsmdbPage(Rows, TotalCount, VersionLabel)
```

## Die Klassen

| Klasse | Aufgabe |
| --- | --- |
| `SsmdbReader` | Einstieg: baut die Verbindung, liest eine Seite einer Tabelle |
| `SsmdbSchemaDetector` | liest vorhandene Spalten aus `information_schema.COLUMNS` |
| `SsmdbSchema` | beantwortet `Has(tabelle, spalte)` und liefert ein `VersionLabel` |
| `SsmdbQueries` | baut die `SELECT`-Statements |
| `SsmdbMapper` | wandelt eine Reader-Zeile in ein Modell |

## Umgang mit Schemaversionen

Die Versionsnummer steht nicht in der Datenbank. Deshalb wird geprüft, **welche Spalten
existieren**:

| Spalte | vorhanden ab |
| --- | --- |
| `Scheiben.StandNrText` | V8 |
| `Treffer.Serie` | V8 |
| `Scheiben.BesterTeiler001` | V9 |

Fehlt eine Spalte, schreibt `SsmdbQueries` stattdessen `NULL AS Spalte` ins `SELECT`. Dadurch
liefert jede Version **dieselben Spalten in derselben Reihenfolge**, und ein einziger Mapper
genügt.

```sql
-- V5: StandNrText fehlt
SELECT `ScheibenID`, …, NULL AS `StandNrText`, … FROM `Scheiben` ORDER BY ScheibenID
```

`VersionLabel` ist nur eine grobe Einordnung für die Anzeige: `V5/V7`, `V8` oder `V9 oder neuer`
(V5 und V7 lassen sich an den Spalten nicht unterscheiden).

## Eine Spalte ergänzen

Drei Stellen müssen zusammenpassen:

1. Property im Modell (`Models/`)
2. Spaltenname in der Liste in `SsmdbQueries` (`ScheibenColumns`, `SerienColumns`, `TrefferColumns`)
3. Zuweisung in `SsmdbMapper` mit dem **passenden Index**

::: danger Reihenfolge = Index
Der Mapper liest über die Position (`Int(r, 4)`). Eine Spalte mitten in die Liste einzufügen
verschiebt alle folgenden Indizes – neue Spalten daher bewusst einsortieren und den Mapper
vollständig nachziehen.
:::

## Sicherheit

- Tabellen- und Spaltennamen kommen aus festen Listen, nie aus Nutzereingaben.
- Werte (`LIMIT`, `OFFSET`, Datenbankname) gehen als Parameter.
- Fehlt eine der drei Tabellen, wirft der Detector eine `SsmdbSchemaException`
  („Ist das eine SSMDB2?").

Weiterführend: [MySqlConnector-Dokumentation](https://mysqlconnector.net/)
