using System.Collections.Generic;
using System.Linq;

namespace ShotPost.Data;

/// <summary>
/// Baut SELECT-Statements, die in JEDER Schemaversion dieselben Spalten in derselben Reihenfolge
/// liefern: fehlende Spalten kommen als "NULL AS Spalte" zurueck. So reicht ein einziger Mapper.
/// Alle Namen stammen aus festen Listen (kein Nutzer-Input im SQL).
/// </summary>
public static class SsmdbQueries
{
    // Reihenfolge = Ordinal im Reader (siehe SsmdbMapper).
    public static readonly string[] ScheibenColumns =
    [
        "ScheibenID", "Starterliste", "StarterlistenID", "Startzeit", "StartNr",
        "Nachname", "Vorname", "Geschlecht", "Land", "SportpassID",
        "StandNr", "StandNrText", "Disziplin", "DisziplinID", "Klasse", "KlassenID",
        "Verein", "VereinsID", "Mannschaft", "MannschaftsID", "Rangliste", "RanglistenID",
        "Gruppe", "GruppenID", "Status", "Trefferzahl", "TotalRing", "TotalRing01",
        "BesterTeiler01", "BesterTeiler001", "Zeitstempel"
    ];

    public static readonly string[] SerienColumns =
    [
        "ScheibenID", "Stellung", "Serie", "Ring", "Ring01"
    ];

    public static readonly string[] TrefferColumns =
    [
        "ScheibenID", "Stellung", "Treffer", "Serie", "x", "y", "Innenzehner", "Wertung",
        "Text", "Ring", "Ring01", "Teiler01", "Teiler001", "Zeitstempel", "Millisekunden"
    ];

    /// <param name="whereClause">Optional, z.B. "Zeitstempel &gt; @seit". Parameter setzt der Aufrufer.</param>
    public static string Scheiben(SsmdbSchema schema, string? whereClause = null)
        => Build(schema, SsmdbSchema.TabelleScheiben, ScheibenColumns, whereClause, "ScheibenID");

    public static string Serien(SsmdbSchema schema, string? whereClause = null)
        => Build(schema, SsmdbSchema.TabelleSerien, SerienColumns, whereClause, "ScheibenID, Stellung, Serie");

    public static string Treffer(SsmdbSchema schema, string? whereClause = null)
        => Build(schema, SsmdbSchema.TabelleTreffer, TrefferColumns, whereClause, "ScheibenID, Stellung, Treffer");

    private static string Build(
        SsmdbSchema schema, string table, IEnumerable<string> columns, string? where, string orderBy)
    {
        var select = string.Join(", ", columns.Select(c =>
            schema.Has(table, c) ? $"`{c}`" : $"NULL AS `{c}`"));

        var sql = $"SELECT {select} FROM `{table}`";
        if (!string.IsNullOrWhiteSpace(where)) sql += $" WHERE {where}";
        return sql + $" ORDER BY {orderBy}";
    }
}
