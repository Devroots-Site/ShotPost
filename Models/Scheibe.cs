using System;

namespace ShotPost.Models;

/// <summary>
/// Tabelle "Scheiben" (SSMDB2). Entspricht dem Schema von V9 (Obermenge aller Versionen).
/// Spalten, die in aelteren Versionen fehlen, bleiben <c>null</c>:
///   StandNrText     ab V8
///   BesterTeiler001 ab V9
/// </summary>
public sealed class Scheibe
{
    public int ScheibenID { get; set; }
    public string? Starterliste { get; set; }
    public int? StarterlistenID { get; set; }
    public DateTime? Startzeit { get; set; }
    public int? StartNr { get; set; }
    public string? Nachname { get; set; }
    public string? Vorname { get; set; }
    /// <summary>'male', 'female' oder 'unknown'.</summary>
    public string? Geschlecht { get; set; }
    /// <summary>IOC-Landeskuerzel.</summary>
    public string? Land { get; set; }
    /// <summary>int(11) oder bigint(20) je nach Version -> immer long.</summary>
    public long? SportpassID { get; set; }
    public short? StandNr { get; set; }
    /// <summary>Ab V8: alphanumerische Standnummer (z.B. "H" bei ISSF Finals).</summary>
    public string? StandNrText { get; set; }
    public string? Disziplin { get; set; }
    public int? DisziplinID { get; set; }
    public string? Klasse { get; set; }
    public int? KlassenID { get; set; }
    public string? Verein { get; set; }
    public int? VereinsID { get; set; }
    public string? Mannschaft { get; set; }
    public int? MannschaftsID { get; set; }
    public string? Rangliste { get; set; }
    public int? RanglistenID { get; set; }
    public string? Gruppe { get; set; }
    public int? GruppenID { get; set; }
    /// <summary>z.B. RWK, Liga, Heim, Gast, S, E, F, AK, Einlage, GMM, BMM, I.</summary>
    public string? Status { get; set; }
    public short? Trefferzahl { get; set; }
    public int? TotalRing { get; set; }
    public int? TotalRing01 { get; set; }
    public int? BesterTeiler01 { get; set; }
    /// <summary>Ab V9.</summary>
    public int? BesterTeiler001 { get; set; }
    public DateTime? Zeitstempel { get; set; }
}
