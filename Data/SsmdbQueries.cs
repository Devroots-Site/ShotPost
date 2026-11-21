using System.Collections.Generic;
using System.Linq;

namespace ShotPost.Data;

/// <summary>
/// Eine Spalte im SELECT. <see cref="Alias"/> nur setzen, wenn die Property im Modell anders heisst
/// als die Spalte in der Datenbank (Dapper mappt ueber den Namen).
/// </summary>
public sealed record SsmdbColumn(string Name, string? Alias = null)
{
    public static implicit operator SsmdbColumn(string name) => new(name);

    /// <summary>Name, unter dem die Spalte im Ergebnis erscheint.</summary>
    public string ResultName => Alias ?? Name;
}

/// <summary>
/// Baut SELECT-Statements, die in JEDER Schemaversion dieselben Spaltennamen liefern: fehlende
/// Spalten kommen als "NULL AS Spalte" zurueck. So mappt Dapper alle Versionen auf dieselben Modelle.
/// Alle Namen stammen aus festen Listen (kein Nutzer-Input im SQL).
/// </summary>
public static class SsmdbQueries
{
    public static readonly SsmdbColumn[] ScheibenColumns =
    [
        "ScheibenID",
        "Starterliste",
        "StarterlistenID",
        "Startzeit",
        "StartNr",
        "Nachname",
        "Vorname",
        "Geschlecht",
        "Land",
        "SportpassID",
        "StandNr",
        "StandNrText",
        "Disziplin",
        "DisziplinID",
        "Klasse",
        "KlassenID",
        "Verein",
        "VereinsID",
        "Mannschaft",
        "MannschaftsID",
        "Rangliste",
        "RanglistenID",
        "Gruppe",
        "GruppenID",
        "Status",
        "Trefferzahl",
        "TotalRing",
        "TotalRing01",
        "BesterTeiler01",
        "BesterTeiler001",
        "Zeitstempel",
    ];

    public static readonly SsmdbColumn[] SerienColumns =
    [
        "ScheibenID",
        "Stellung",
        new("Serie", "SerieNr"),
        "Ring",
        "Ring01",
    ];

    public static readonly SsmdbColumn[] TrefferColumns =
    [
        "ScheibenID",
        "Stellung",
        new("Treffer", "TrefferNr"),
        "Serie",
        "x",
        "y",
        "Innenzehner",
        "Wertung",
        "Text",
        "Ring",
        "Ring01",
        "Teiler01",
        "Teiler001",
        "Zeitstempel",
        "Millisekunden",
    ];

    /// <param name="whereClause">Optional, z.B. "Zeitstempel &gt; @seit". Parameter setzt der Aufrufer.</param>
    public static string Scheiben(SsmdbSchema schema, string? whereClause = null) =>
        Build(schema, SsmdbSchema.TabelleScheiben, ScheibenColumns, whereClause, "ScheibenID");

    public static string Serien(SsmdbSchema schema, string? whereClause = null) =>
        Build(
            schema,
            SsmdbSchema.TabelleSerien,
            SerienColumns,
            whereClause,
            "ScheibenID, Stellung, Serie"
        );

    public static string Treffer(SsmdbSchema schema, string? whereClause = null) =>
        Build(
            schema,
            SsmdbSchema.TabelleTreffer,
            TrefferColumns,
            whereClause,
            "ScheibenID, Stellung, Treffer"
        );

    private static string Build(
        SsmdbSchema schema,
        string table,
        IEnumerable<SsmdbColumn> columns,
        string? where,
        string orderBy
    )
    {
        var select = string.Join(", ", columns.Select(c => Select(schema, table, c)));

        var sql = $"SELECT {select} FROM `{table}`";
        if (!string.IsNullOrWhiteSpace(where))
            sql += $" WHERE {where}";
        return sql + $" ORDER BY {orderBy}";
    }

    private static string Select(SsmdbSchema schema, string table, SsmdbColumn c)
    {
        if (!schema.Has(table, c.Name))
            return $"NULL AS `{c.ResultName}`";
        return c.Alias is null ? $"`{c.Name}`" : $"`{c.Name}` AS `{c.Alias}`";
    }
}
