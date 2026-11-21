# Payload

`Services/PayloadBuilder.cs` baut aus einem Modell das Objekt, das an den Server gehen soll –
nur mit den in den Einstellungen ausgewählten Feldern.

::: warning Bisher nur Vorschau
Der Versand selbst ist noch nicht implementiert. Der `PayloadBuilder` wird aktuell für die
JSON-Vorschau im Reiter „Daten" der Einstellungen genutzt.
:::

## Funktionen

| Methode | Aufgabe |
| --- | --- |
| `GetFieldNames(Type)` | alle sendbaren Felder eines Modells, in Deklarationsreihenfolge |
| `IsKeyField(Type, string)` | ist das Feld ein Schlüsselfeld? |
| `Build<T>(item, selectedFields)` | Dictionary aus Feldname → Wert; `null` = alle Felder |

## Schlüsselfelder

Schlüsselfelder werden **immer** gesendet, sonst ließen sich Datensätze nicht zuordnen.
In der Oberfläche sind sie nicht abwählbar.

| Modell | Schlüsselfelder |
| --- | --- |
| `Scheibe` | `ScheibenID` |
| `Serie` | `ScheibenID`, `Stellung`, `SerieNr` |
| `Treffer` | `ScheibenID`, `Stellung`, `TrefferNr` |

## Zusammenspiel mit den Einstellungen

- `DataTableViewModel` stellt je Tabelle die Feldliste mit Häkchen dar
  (`FieldOptionViewModel` je Feld).
- Jede Änderung löst `SettingsViewModel.UpdatePayloadPreview` aus.
- Die Vorschau nutzt leere Beispieldatensätze und zeigt damit die **Struktur**:

```json
{
  "Scheiben": [{ "ScheibenID": 0, "Nachname": null }],
  "Serien": [{ "ScheibenID": 0, "Stellung": 0, "SerieNr": 0 }]
}
```

Ein neues Modell-Feld erscheint ohne weiteres Zutun in Auswahl und Vorschau –
siehe [Modelle](./modelle).
