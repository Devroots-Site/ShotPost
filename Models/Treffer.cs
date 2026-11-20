using System;

namespace ShotPost.Models;

/// <summary>
/// Tabelle "Treffer". PK: ScheibenID, Stellung, Treffer.
/// Ab V8 zusaetzlich die Spalte "Serie"; dort ist Stellung 0 die Probestellung.
/// </summary>
public sealed class Treffer
{
    public int ScheibenID { get; set; }
    public short Stellung { get; set; }
    /// <summary>Fortlaufende Nummer des Treffers innerhalb der Stellung (Spalte "Treffer").</summary>
    public short TrefferNr { get; set; }
    /// <summary>Ab V8: Nummer der Serie, zu der der Treffer gehoert.</summary>
    public short? Serie { get; set; }
    /// <summary>x-Koordinate in 1/100 mm, Ursprung = Scheibenmittelpunkt.</summary>
    public int? X { get; set; }
    /// <summary>y-Koordinate in 1/100 mm, Ursprung = Scheibenmittelpunkt.</summary>
    public int? Y { get; set; }
    public bool? Innenzehner { get; set; }
    /// <summary>Wertungsmodus 0..12, siehe <see cref="Wertungsmodus"/>.</summary>
    public int? Wertung { get; set; }
    public string? Text { get; set; }
    public int? Ring { get; set; }
    public int? Ring01 { get; set; }
    public int? Teiler01 { get; set; }
    public int? Teiler001 { get; set; }
    public DateTime? Zeitstempel { get; set; }
    public short? Millisekunden { get; set; }

    /// <summary>Probestellung (ab V8: Stellung = 0).</summary>
    public bool IstProbe => Stellung == 0;
}

/// <summary>Werte der Spalte Treffer.Wertung (in allen Versionen gleich).</summary>
public enum Wertungsmodus
{
    Unbekannt = 0,
    ZehntelRinge = 1,
    GanzeRinge = 2,
    Teiler = 3,
    Trefferumkreis = 4,
    KomplettVerdeckt = 5,
    Schweizer100er = 6,
    ZehntelRingePlusBesterTeiler = 7,
    GanzeRingePlusBesterTeiler = 8,
    ZehntelTeiler = 9,
    Verdeckt = 10,
    HundertstelTeiler = 11,
    Nachkommastelle = 12,
}
