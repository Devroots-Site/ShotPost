using System;
using System.Collections.Generic;

namespace ShotPost.Data;

/// <summary>
/// Beschreibt, welche Spalten in der angebundenen SSMDB2 tatsaechlich existieren.
/// Statt nach Versionsnummer zu verzweigen (die steht nicht in der Datenbank), wird
/// per Feature-Erkennung gearbeitet. Das deckt auch unbekannte Zwischenversionen ab.
/// </summary>
public sealed class SsmdbSchema
{
    public const string TabelleScheiben = "Scheiben";
    public const string TabelleSerien = "Serien";
    public const string TabelleTreffer = "Treffer";

    private readonly Dictionary<string, HashSet<string>> _columns;

    public SsmdbSchema(string databaseName, Dictionary<string, HashSet<string>> columns)
    {
        DatabaseName = databaseName;
        _columns = new Dictionary<string, HashSet<string>>(
            columns,
            StringComparer.OrdinalIgnoreCase
        );
    }

    public string DatabaseName { get; }

    public bool Has(string table, string column) =>
        _columns.TryGetValue(table, out var cols) && cols.Contains(column);

    // Aenderungen gegenueber V5
    public bool HasStandNrText => Has(TabelleScheiben, "StandNrText"); // ab V8
    public bool HasTrefferSerie => Has(TabelleTreffer, "Serie"); // ab V8
    public bool HasBesterTeiler001 => Has(TabelleScheiben, "BesterTeiler001"); // ab V9

    /// <summary>Grobe Einordnung fuer Logs/Anzeige. V5 und V7 sind an den Spaltennamen nicht unterscheidbar.</summary>
    public string VersionLabel =>
        HasBesterTeiler001 ? "V9 oder neuer"
        : HasStandNrText || HasTrefferSerie ? "V8"
        : "V5/V7";
}
