namespace ShotPost.Models;

/// <summary>Tabelle "Serien". PK: ScheibenID, Stellung, Serie. In allen Versionen identisch.</summary>
public sealed class Serie
{
    public int ScheibenID { get; set; }
    public short Stellung { get; set; }
    /// <summary>Fortlaufende Nummer der Serie innerhalb einer Stellung.</summary>
    public short SerieNr { get; set; }
    public int? Ring { get; set; }
    public int? Ring01 { get; set; }
}
