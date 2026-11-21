# Modelle

Unter `Models/`. Die drei Datenmodelle bilden je eine Tabelle der SSMDB2 ab und orientieren sich
am Schema von **V9** (Obermenge aller Versionen). Spalten, die es in einer älteren Version nicht
gibt, bleiben `null`.

## Scheibe

Tabelle `Scheiben` – ein Ergebnis je Schütze und Scheibe. Schlüssel: `ScheibenID`.

Enthält Angaben zum Schützen (Name, Verein, Klasse, Mannschaft …), zum Stand und das
Gesamtergebnis (`TotalRing`, `TotalRing01`, `BesterTeiler01`).

- `StandNrText` gibt es ab V8, `BesterTeiler001` ab V9.
- `SportpassID` ist je nach Version `int` oder `bigint` – im Modell immer `long`.

## Serie

Tabelle `Serien` – Summen je Serie. Schlüssel: `ScheibenID`, `Stellung`, `SerieNr`.
In allen Versionen identisch.

::: info Abweichende Namen
Die Spalte heißt `Serie`, die Property `SerieNr`. Ebenso bei Treffern: Spalte `Treffer`,
Property `TrefferNr`. `SsmdbQueries` benennt diese Spalten im `SELECT` per Alias um
(`` `Serie` AS `SerieNr` ``), damit Dapper sie zuordnen kann.
:::

## Treffer

Tabelle `Treffer` – jeder einzelne Schuss. Schlüssel: `ScheibenID`, `Stellung`, `TrefferNr`.

- `X` / `Y`: Koordinaten in 1/100 mm, Ursprung ist der Scheibenmittelpunkt.
- `Serie` gibt es ab V8.
- `IstProbe`: `Stellung == 0` ist die Probestellung (ab V8).
- `Wertung`: Wertungsmodus 0–12, siehe Enum `Wertungsmodus` in derselben Datei.

## AppSettings

Alle vom Benutzer einstellbaren Werte; wird als JSON gespeichert.
Details unter [Einstellungen & Theme](./einstellungen).

::: tip Properties sind die Feldliste
Der [`PayloadBuilder`](./payload) liest die sendbaren Felder per Reflection aus den Modellen:
jede öffentliche Property mit öffentlichem Setter erscheint automatisch in der Feldauswahl der
Einstellungen. Berechnete Properties ohne Setter (wie `IstProbe`) werden nicht gesendet.
:::
